using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum BattleState {SelectingCommand, SelectingTarget}
public class CommandManager : SingletonBase<CommandManager>
{
    [HideInInspector] public Unit[] allyInstances;
    [HideInInspector] public Unit[] enemyInstances;
    [HideInInspector] public Unit activeUnit;

    [HideInInspector] public int selectedAction;
    [SerializeField] private UnitCursor unitCursor;
    private Unit commandTarget = null;
    private BattleState battleState;

    public void Start()
    {
        allyInstances = UnitManager.Instance.allies;
        enemyInstances = UnitManager.Instance.currentEnemies;
        SetActiveUnit();
        unitCursor = Instantiate(unitCursor) as UnitCursor;
        unitCursor.FollowNewUnit(activeUnit);
        battleState = BattleState.SelectingCommand;
    }


    private void Update() 
    {
        if(battleState == BattleState.SelectingCommand)
        {
            if(!activeUnit || activeUnit.state == UnitStates.CannotAction)
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
                        StartCoroutine(SelectCommandTarget(activeUnit.commands[selectedAction].action.GetTargetPool()));
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
            if(allyInstances[i].state == UnitStates.CanAction)
            {
                activeUnit = allyInstances[i];
                selectedAction = 0;
                MenuManager.Instance.FillMenu(activeUnit);
                unitCursor.gameObject.SetActive(true);
                unitCursor.FollowNewUnit(activeUnit);
                return;
            }
        }
        activeUnit = null;
        unitCursor.gameObject.SetActive(false);
    }

    public IEnumerator SelectCommandTarget(Unit[] targetPool)
    {
        MenuManager.Instance.SetMenuActive(false);
        battleState = BattleState.SelectingTarget;
        int selectedTarget = 0;
        unitCursor.FollowNewUnit(targetPool[selectedTarget]); 
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
                unitCursor.FollowNewUnit(targetPool[selectedTarget]);
            }
            else if(Input.GetKeyDown(KeyCode.W) && selectedTarget > 0)
            {
                selectedTarget--;
                unitCursor.FollowNewUnit(targetPool[selectedTarget]);
            }
            yield return null;
        }
        battleState = BattleState.SelectingCommand;
        commandTarget = targetPool[selectedTarget];
        MenuManager.Instance.SetMenuActive(true);
        activeUnit.ExecuteAction(activeUnit.commands[selectedAction], commandTarget);
    }
}