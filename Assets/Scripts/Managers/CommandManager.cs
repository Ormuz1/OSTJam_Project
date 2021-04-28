using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum CommandMenuState {SelectingCommand, SelectingTarget}
public class CommandManager : SingletonBase<CommandManager>
{
    [HideInInspector] public Ally[] allyInstances;
    [HideInInspector] public Unit[] enemyInstances;
    [HideInInspector] public Ally activeUnit;
    [HideInInspector] public int selectedAction;
    [SerializeField] private UnitCursor unitCursor;
    private Unit commandTarget = null;
    public CommandMenuState commandMenuState;
    private int lastActiveUnitIndex = 0;
    public override void Awake()
    {
        base.Awake();
        unitCursor = Instantiate(unitCursor) as UnitCursor;
    }

    private void Start() 
    {
        ResetCommandMenu();    
    }

    public void ResetCommandMenu()
    {
        StopAllCoroutines();
        allyInstances = UnitManager.Instance.allies;
        enemyInstances = UnitManager.Instance.currentEnemies;
        SetActiveUnit();
        unitCursor.gameObject.SetActive(true);
        unitCursor.FollowNewUnit(activeUnit);
        commandMenuState = CommandMenuState.SelectingCommand;
    }


    public void DisableCommandMenu()
    {
        StopAllCoroutines();
        activeUnit = null;
        unitCursor.gameObject.SetActive(false);
        gameObject.SetActive(false);
        MenuManager.Instance.SetMenuActive(false);
    }


    private void Update() 
    {
        if(commandMenuState == CommandMenuState.SelectingCommand)
        {
            if(!activeUnit || activeUnit.state != UnitStates.CanAction)
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

    public void SetActiveUnit()
    {
        if(lastActiveUnitIndex == allyInstances.Length - 1)
            lastActiveUnitIndex = 0;
        for(int i = lastActiveUnitIndex; i < allyInstances.Length; i++)
        {
            if(allyInstances[i].state == UnitStates.CanAction)
            {
                activeUnit = allyInstances[i];
                selectedAction = 0;
                MenuManager.Instance.FillMenu(activeUnit);
                unitCursor.gameObject.SetActive(true);
                unitCursor.FollowNewUnit(activeUnit);
                lastActiveUnitIndex = i;
                return;
            }
        }
        lastActiveUnitIndex = 0;
        activeUnit = null;
        unitCursor.gameObject.SetActive(false);
        StopAllCoroutines();
        CommandManager.Instance.commandMenuState = CommandMenuState.SelectingCommand;
    }

    public IEnumerator SelectCommandTarget(Unit[] targetPool)
    {
        MenuManager.Instance.SetMenuActive(false);
        commandMenuState = CommandMenuState.SelectingTarget;
        int selectedTarget = 0;
        unitCursor.FollowNewUnit(targetPool[selectedTarget]); 
        yield return null;        
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                commandTarget = null;
                MenuManager.Instance.SetMenuActive(true);
                unitCursor.FollowNewUnit(activeUnit);
                commandMenuState = CommandMenuState.SelectingCommand;
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
        commandTarget = targetPool[selectedTarget];
        activeUnit.ExecuteAction(activeUnit.commands[selectedAction], commandTarget);
        MenuManager.Instance.SetMenuActive(false);
        commandMenuState = CommandMenuState.SelectingCommand;
        unitCursor.gameObject.SetActive(false);
    }
}