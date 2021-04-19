using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleEnemy : Unit
{
    [SerializeField] private UnitCommand[] enemyInstructions;
    private int currentInstruction = 0;
    private CommandManager battleManager;

    protected void Start()
    {
        battleManager = FindObjectOfType<CommandManager>();
    }
    private void Update() 
    {
        if(state == UnitStates.CanAction)
        {
            Debug.Log("Enemy Attacking");
            ExecuteAction(enemyInstructions[currentInstruction], battleManager.allyInstances[Random.Range(0, battleManager.allyInstances.Length)]);
            currentInstruction = currentInstruction + 1 < enemyInstructions.Length ? currentInstruction + 1 : 0;
        }
    }
}
