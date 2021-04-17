using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BattleState {SelectingCommand, SelectingTarget}
public class BattleManager : MonoBehaviour
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
    [SerializeField] private MenuManager menu;
    [SerializeField] private GameObject activeUnitCursor;
    private Unit commandTarget = null;
    private BattleState battleState;

    public void Awake()
    {
        allyInstances = InstantiateUnits(allies, alliesRow, spaceBetweenAllies);
        enemyInstances = InstantiateUnits(enemies, enemyFrontRow, spaceBetweenEnemies);
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
                menu.SetMenuActive(false);
                SetActiveUnit();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if(activeUnit.actions[selectedAction].action.RequiresTarget())
                    {
                        StartCoroutine(SelectCommandTarget(enemyInstances.Concat(allyInstances).ToArray()));
                    }
                    else
                    {
                        activeUnit.ExecuteAction(activeUnit.actions[selectedAction]);
                    }
                }
                else if(Input.GetKeyDown(KeyCode.S) && selectedAction < activeUnit.actions.Length -1)
                {
                    selectedAction++;
                    menu.selectedAction = selectedAction;
                    menu.CalculateCursorPosition();
                }
                else if(Input.GetKeyDown(KeyCode.W) && selectedAction > 0)
                {
                    selectedAction--;
                    menu.selectedAction = selectedAction;
                    menu.CalculateCursorPosition();
                }
            }
        }
    }

    private Unit[] InstantiateUnits(Unit[] units, Vector3 centerPoint, Vector3 spaceBetweenUnits)
    {
        Unit[] unitInstances = new Unit[units.Length];
        for (int i = 0; i < units.Length; i++)
        {
        
            Vector3 newUnitPosition = new Vector3(
                (centerPoint.x + spaceBetweenUnits.x * i) - spaceBetweenUnits.x * units.Length * 0.5f + spaceBetweenUnits.x * 0.5f,
                (centerPoint.y + spaceBetweenUnits.y * i) - spaceBetweenUnits.y * units.Length * 0.5f + spaceBetweenUnits.y * 0.5f,
                (centerPoint.z + spaceBetweenUnits.z * i) - spaceBetweenUnits.z * units.Length * 0.5f + spaceBetweenUnits.z * 0.5f
            );
            Unit unitInstance = Instantiate(units[i], newUnitPosition, Quaternion.identity) as Unit;
            Debug.Log(newUnitPosition);
            unitInstance.transform.parent = transform;
            unitInstances[i] = unitInstance;
        }
        return unitInstances;
    }


    private void SetActiveUnit()
    {
        for(int i = 0; i < allyInstances.Length; i++)
        {
            if(allyInstances[i].state == UnitStates.Idle)
            {
                activeUnit = allyInstances[i];
                selectedAction = 0;
                menu.FillMenu(activeUnit.actions);
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
        menu.SetMenuActive(false);
        battleState = BattleState.SelectingTarget;
        int selectedTarget = 0;
        activeUnitCursor.transform.position = targetPool[selectedTarget].transform.position + Vector3.up * 1;
        yield return null;
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                commandTarget = null;
                menu.SetMenuActive(true);
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
        menu.SetMenuActive(true);
        activeUnit.ExecuteAction(activeUnit.actions[selectedAction], commandTarget);
    }
}