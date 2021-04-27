using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Target {Random, LowestHealth, HighestHealth}
public enum TargetPool {Allies, Enemies, All}
public class SimpleEnemy : Unit
{
    [System.Serializable]
    protected class EnemyInstruction
    {
        public UnitCommand command;
        public TargetPool targetPool;
        public Target target;
        public float timeToExecute;
    }
    [SerializeField] private EnemyInstruction[] enemyInstructions;
    [SerializeField] private float waitTimeModifierRange = 0.2f;
    private int currentInstruction = 0;
    private float commandTimer = 0;
    private bool shouldDrawTimer = true;
    private Unit nextTarget;
    public const float ATTACK_ANIMATION_WINDUP_TIME = 1.6515f;
    private float currenActionWaitTime;
    private void Update() 
    {
        if(state == UnitStates.CanAction)
        {
            if(shouldDrawTimer)
            {
                currenActionWaitTime = enemyInstructions[currentInstruction].timeToExecute + Random.Range(-waitTimeModifierRange, waitTimeModifierRange);
                nextTarget = ChooseTarget(enemyInstructions[currentInstruction]);
                currentRadialTimer = MenuManager.Instance.DrawRadialTimer(currenActionWaitTime, this);
                shouldDrawTimer = false;
                
            }
            
            if(!nextTarget || !nextTarget.CanBeTargeted)
            {
                nextTarget = ChooseTarget(enemyInstructions[currentInstruction]);
                if(nextTarget == null)
                {
                    state = UnitStates.CannotAction;
                    commandTimer = 0;
                    shouldDrawTimer = true;
                    Destroy(currentRadialTimer.gameObject);
                    StartCoroutine(WaitForAvailableTargets()); 
                }
            }
            else if(commandTimer < currenActionWaitTime)
            {
                commandTimer += Time.deltaTime;
            }
            else
            {
                ExecuteEnemyInstruction(enemyInstructions[currentInstruction]);
                currentInstruction = currentInstruction + 1 < enemyInstructions.Length ? currentInstruction + 1 : 0;
                commandTimer = 0;
                if(enemyInstructions[currentInstruction].command.action != UnitActions.Stop)
                    shouldDrawTimer = true;
            }
        }
    }

    protected void ExecuteEnemyInstruction(EnemyInstruction instruction)
    {
        ExecuteAction(instruction.command, nextTarget);
    }

    private IEnumerator WaitForAvailableTargets()
    {
        while(!UnitManager.Instance.allies.Any(ally => ally.CanBeTargeted))
        {
            yield return new WaitForSeconds(1);
        }
        state = UnitStates.CanAction;
    }

    private Unit ChooseTarget(EnemyInstruction instruction)
    {
        Unit[] targetPool;
        switch(instruction.targetPool)
        {
            case TargetPool.Allies:
                targetPool = UnitManager.Instance.allies;
                break;
            case TargetPool.Enemies:
                targetPool = UnitManager.Instance.currentEnemies;
                break;
            default:
                targetPool = UnitManager.Instance.allies.Concat(UnitManager.Instance.currentEnemies).ToArray();
                break;
        }
        targetPool = targetPool.Where(unit => unit.CanBeTargeted).ToArray();
        if(targetPool.Length == 0)
            return null;
        Unit targetUnit;
        switch(instruction.target)
        {
            case Target.Random:
                targetUnit = targetPool[Random.Range(0, targetPool.Length)];
                break;
            case Target.LowestHealth:
                targetUnit = targetPool.GetLowestHealth();
                break;
            default:
                targetUnit = targetPool.GetHighestHealth();
                break;
        }
        return targetUnit;
    }  
    protected override void OnDeath()
    {
        base.OnDeath();
        StartCoroutine(CheckForEndOfEncounter());

    }


    private IEnumerator CheckForEndOfEncounter()
    {
        yield return new WaitForSeconds(deathAnimation.length);
        UnitManager.Instance.currentEnemies = UnitManager.Instance.currentEnemies.Where(item => item != this).ToArray();
        if(UnitManager.Instance.currentEnemies.Length == 0)
        {
            UnitManager.Instance.StartCoroutine(UnitManager.Instance.GoToNextEncounter());
        }
        Destroy(gameObject);
    }
}
