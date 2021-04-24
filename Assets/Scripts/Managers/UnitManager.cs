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
        public float encounterTime = 10f;
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
    [SerializeField] private float timeBetweenEncounters = 5f;
    private int currentEncounter = 0;
    [HideInInspector] public Unit[] currentEnemies;
    private readonly Vector3 spaceBetweenEncounters = new Vector3(0, 0, 10);
    private CameraFollow cameraFollowScript;

    public override void Awake()
    {
        base.Awake();
        cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
        Ally[] existingAllies = FindObjectsOfType<Ally>();
        if(existingAllies.Length == 0)
            allies = CreateUnits(allies, alliesStartingPoint, spaceBetweenAllies).Select(ally => ally as Ally).ToArray();
        else
            allies = existingAllies;
        currentEnemies = CreateUnits(enemyEncounters[currentEncounter].enemies, enemyFrontRow, spaceBetweenEnemies);
        TimerManager.Instance.StartTimer(enemyEncounters[currentEncounter].encounterTime);
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
            CommandManager.Instance.DisableCommandMenu();
            MenuManager.Instance.SetLifeBarMenuActive(false);
            Coroutine lastCoroutine = null;
            for(int i = 0; i < allies.Length; i++)
            {
                lastCoroutine = allies[i].StartCoroutine(CommandCoroutines.MoveToPosition(allies[i], allies[i].transform.position + spaceBetweenEncounters, timeBetweenEncounters));
                allies[i].state = UnitStates.CannotAction;
            }
            TimerManager.Instance.StartCoroutine(TimerManager.Instance.RestartTimer(timeBetweenEncounters, enemyEncounters[currentEncounter].encounterTime));
            cameraFollowScript.isFollowing = true;
            currentEnemies = CreateUnits(enemyEncounters[currentEncounter].enemies, enemyFrontRow + spaceBetweenEncounters * currentEncounter, spaceBetweenEnemies, UnitStates.CannotAction);
            yield return lastCoroutine;
            for(int i = 0; i < allies.Length; i++)
            {
                allies[i].state = UnitStates.CanAction;
            }
            cameraFollowScript.isFollowing = false;
            for(int i = 0; i < currentEnemies.Length; i++)
            {
                currentEnemies[i].state = UnitStates.CanAction;
            }
            MenuManager.Instance.SetLifeBarMenuActive(true);
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
