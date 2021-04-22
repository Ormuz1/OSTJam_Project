using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum BattleState {Active, Transitioning}
public class UnitManager : SingletonBase<UnitManager>
{
    [System.Serializable]
    private class Encounter
    {
        public Unit[] enemies;
        public Encounter(Unit[] enemies)
        {
            this.enemies = enemies;
        }
    }

    [SerializeField] private Vector3 alliesStartingPoint;
    [SerializeField] private Vector3 enemyFrontRow;
    [SerializeField] private Vector3 spaceBetweenAllies;
    [SerializeField] private Vector3 spaceBetweenEnemies;
    public Ally[] allies;
    [SerializeField] private Encounter[] enemyEncounters;
    private int currentEncounter = 0;
    [HideInInspector] public Unit[] currentEnemies;
    private readonly Vector3 spaceBetweenEncounters = new Vector3(0, 0, 10);

    public override void Awake()
    {
        base.Awake();
        allies = CreateUnits(allies, alliesStartingPoint, spaceBetweenAllies).Select(parent => parent as Ally).ToArray();
        currentEnemies = CreateUnits(enemyEncounters[currentEncounter].enemies, enemyFrontRow, spaceBetweenEnemies);
    }

    public Unit[] CreateUnits(Unit[] units, Vector3 centerPoint, Vector3 spaceBetweenUnits, UnitStates startState = UnitStates.CanAction)
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
            unitInstance.transform.parent = transform;
            unitInstance.state = startState;
            unitInstances[i] = unitInstance;
        }

        return unitInstances;
    }

    public IEnumerator GoToNextEncounter()
    {
        currentEncounter++;
        if(currentEncounter < enemyEncounters.Length)
        {
            yield return WaitForUnitsIdle(allies);
            CommandManager.Instance.gameObject.SetActive(false);
            Coroutine lastCoroutine = null;
            for(int i = 0; i < allies.Length; i++)
            {
                Debug.Log("Running " + i.ToString());
                lastCoroutine = allies[i].StartCoroutine(CommandCoroutines.MoveToPosition(allies[i], allies[i].transform.position + spaceBetweenEncounters, 5f));
                allies[i].state = UnitStates.CannotAction;
            }
            currentEnemies = CreateUnits(enemyEncounters[currentEncounter].enemies, enemyFrontRow + spaceBetweenEncounters * currentEncounter, spaceBetweenEnemies, UnitStates.CannotAction);
            yield return lastCoroutine;
            Debug.Log("Player finished moving");
            for(int i = 0; i < currentEnemies.Length; i++)
            {
                currentEnemies[i].state = UnitStates.CanAction;
            }
            CommandManager.Instance.gameObject.SetActive(true);
            CommandManager.Instance.ResetCommandMenu();
        }
        else
        {
            Debug.Log("No more encounters");
        }
    }

    public IEnumerator WaitForUnitsIdle(Unit[] units)
    {
        for(int i = 0; i < units.Length; i++)
        {
            if(units[i].state != UnitStates.CanAction)
            {
                yield return new WaitUntil(() => units[i].state == UnitStates.CanAction);
            }
        }
        yield break;
    }
}
