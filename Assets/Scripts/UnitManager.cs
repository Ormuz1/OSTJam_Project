using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : SingletonBase<UnitManager>
{
    [SerializeField] private Vector3 alliesStartingPoint;
    [SerializeField] private Vector3 enemyFrontRow;
    public Unit[] enemies;
    public Unit[] currentEnemies;
    [SerializeField] private Vector3 spaceBetweenAllies;
    [SerializeField] private Vector3 spaceBetweenEnemies;
    public Unit[] allies;


    public override void Awake()
    {
        base.Awake();
        allies = CreateUnits(allies, alliesStartingPoint, spaceBetweenAllies);
        currentEnemies = CreateUnits(enemies, enemyFrontRow, spaceBetweenEnemies);
    }

    public Unit[] CreateUnits(Unit[] units, Vector3 centerPoint, Vector3 spaceBetweenUnits)
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
            unitInstances[i] = unitInstance;
        }
        return unitInstances;
    }
}
