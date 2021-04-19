using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum BattleState {SelectingCommand, SelectingTarget}
public class BattleManager : SingletonBase<BattleManager>
{
    [SerializeField] private Vector3 alliesRow;
    [SerializeField] private Vector3 enemyFrontRow;
    public Unit[] allies;
    [HideInInspector] public Unit[] allyInstances;
    public Unit[] enemies;
    [HideInInspector] public Unit[] enemyInstances;
    [HideInInspector] public Unit activeUnit;
    [SerializeField] private Vector3 spaceBetweenAllies;
    [SerializeField] private Vector3 spaceBetweenEnemies;

    [HideInInspector] public int selectedAction;
    [SerializeField] private GameObject activeUnitCursor;
    private Unit commandTarget = null;
    private BattleState battleState;

    public void Start()
    {
        allyInstances = UnitManager.Instance.allies;
        enemyInstances = UnitManager.Instance.currentEnemies;
        SetActiveUnit();
        activeUnitCursor = Instantiate(activeUnitCursor, activeUnit.transform.position + Vector3.up * 1, Quaternion.identity);
        battleState = BattleState.SelectingCommand;
    }


    private void Update() 
    {
        if(battleState == BattleState.SelectingCommand)
        {
            if(!activeUnit || activeUnit.state == UnitStates.Busy)
            {
                MenuManager.Instance.SetMenuActive(false);
                SetActiveUnit();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if(activeUnit.commands[selectedAction].action.RequiresTarget())
                    {
                        StartCoroutine(SelectCommandTarget(enemyInstances.Concat(allyInstances).ToArray()));
                    }
                    else
                    {
                        activeUnit.ExecuteAction(activeUnit.commands[selectedAction]);
                    }
                }
                else if(Input.GetKeyDown(KeyCode.S) && selectedAction < activeUnit.commands.Length -1)
                {
                    selectedAction++;
                    MenuManager.Instance.selectedAction = selectedAction;
                    MenuManager.Instance.CalculateCursorPosition();
                }
                else if(Input.GetKeyDown(KeyCode.W) && selectedAction > 0)
                {
                    selectedAction--;
                    MenuManager.Instance.selectedAction = selectedAction;
                    MenuManager.Instance.CalculateCursorPosition();
                }
            }
        }
    }

    private void SetActiveUnit()
    {
        for(int i = 0; i < allyInstances.Length; i++)
        {
            if(allyInstances[i].state == UnitStates.Idle)
            {
                activeUnit = allyInstances[i];
                selectedAction = 0;
                MenuManager.Instance.FillMenu(activeUnit);
                activeUnitCursor.SetActive(true);
                activeUnitCursor.transform.position = activeUnit.transform.position + Vector3.up * 1;
                return;
            }
        }
        activeUnit = null;
        activeUnitCursor.SetActive(false);
    }

    public IEnumerator SelectCommandTarget(Unit[] targetPool)
    {
        MenuManager.Instance.SetMenuActive(false);
        battleState = BattleState.SelectingTarget;
        int selectedTarget = 0;
        activeUnitCursor.transform.position = targetPool[selectedTarget].transform.position + Vector3.up * 1;
        yield return null;
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                commandTarget = null;
                MenuManager.Instance.SetMenuActive(true);
                battleState = BattleState.SelectingCommand;
                yield break;
            }
            else if(Input.GetKeyDown(KeyCode.S) && selectedTarget < targetPool.Length - 1)
            {
                selectedTarget++;
                activeUnitCursor.transform.position = targetPool[selectedTarget].transform.position + Vector3.up * 1;
            }
            else if(Input.GetKeyDown(KeyCode.W) && selectedTarget > 0)
            {
                selectedTarget--;
                activeUnitCursor.transform.position = targetPool[selectedTarget].transform.position + Vector3.up * 1;
            }
            yield return null;
        }
        battleState = BattleState.SelectingCommand;
        commandTarget = targetPool[selectedTarget];
        MenuManager.Instance.SetMenuActive(true);
        activeUnit.ExecuteAction(activeUnit.commands[selectedAction], commandTarget);
    }
}