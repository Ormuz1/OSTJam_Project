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
    private int currentInstruction = 0;
    private float commandTimer = 0;
    private bool shouldDrawTimer = true;
    private void Update() 
    {
        if(state == UnitStates.CanAction)
        {
            if(shouldDrawTimer)
            {
                currentRadialTimer = MenuManager.Instance.DrawRadialTimer(enemyInstructions[currentInstruction].timeToExecute, this);
                shouldDrawTimer = false;
            }

            if(commandTimer < enemyInstructions[currentInstruction].timeToExecute)
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

        ExecuteAction(instruction.command, targetUnit);
    }

    
    protected override void OnDeath()
    {
        base.OnDeath();
        UnitManager.Instance.currentEnemies = UnitManager.Instance.currentEnemies.Where(item => item != this).ToArray();
        if(UnitManager.Instance.currentEnemies.Length == 0)
        {
            UnitManager.Instance.StartCoroutine(UnitManager.Instance.GoToNextEncounter());
        }
        Destroy(gameObject);
    }
}
