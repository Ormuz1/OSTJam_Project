using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleEnemy : Unit
{
    [System.Serializable]
    private class EnemyInstruction
    {
        public UnitCommand command;
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
                MenuManager.Instance.DrawRadialTimer(enemyInstructions[currentInstruction].timeToExecute, this);
                shouldDrawTimer = false;
            }

            if(commandTimer < enemyInstructions[currentInstruction].timeToExecute)
            {
                commandTimer += Time.deltaTime;
            }
            else
            {
                ExecuteAction(enemyInstructions[currentInstruction].command, UnitManager.Instance.allies[Random.Range(0, UnitManager.Instance.allies.Length)]);
                currentInstruction = currentInstruction + 1 < enemyInstructions.Length ? currentInstruction + 1 : 0;
                commandTimer = 0;
                shouldDrawTimer = true;
            }
        }
    }

    protected override void OnDeath()
    {
        UnitManager.Instance.currentEnemies = UnitManager.Instance.currentEnemies.Where(item => item != this).ToArray();
        if(UnitManager.Instance.currentEnemies.Length == 0)
        {
            UnitManager.Instance.StartCoroutine(UnitManager.Instance.GoToNextEncounter());
        }
        Destroy(gameObject);
    }
}
