using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class UnitActionsExtentions
{
    public static string GetActionName(this UnitActions action)
    {
        return System.Enum.GetName(typeof(UnitActions), action);
    }

    public static bool RequiresTarget(this UnitActions action)
    {
        UnitActions[] actionsThatRequireTargets = new UnitActions[]
        {
            UnitActions.Attack,
            UnitActions.Heal
        };

        return actionsThatRequireTargets.Contains(action);
    }
    public static Unit[] GetTargetPool(this UnitActions action)
    {
        Unit[] targetPool;
        switch(action)
        {
            case UnitActions.Heal:
                targetPool = UnitManager.Instance.allies.Concat(UnitManager.Instance.currentEnemies).ToArray();
                break;
            case UnitActions.Attack:
                targetPool = UnitManager.Instance.currentEnemies.Concat(UnitManager.Instance.allies).ToArray();
                break;
            default:
                targetPool = null;
                break;
        }
        return targetPool.Where(unit => unit.state != UnitStates.KnockedOut).ToArray();
    }
}

public static class BoundsExtentions
{
    public static Vector3[] GetScreenCorners(this Bounds bounds)
    {
         
        Vector3 c = bounds.center;
        Vector3 e = bounds.extents;
 
        Vector3[] worldCorners = new [] {
            new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
        };

        IEnumerable<Vector3> screenCorners = worldCorners.Select(corner => Camera.main.WorldToScreenPoint(corner));
        float maxX = screenCorners.Max(corner => corner.x);
        float minX = screenCorners.Min(corner => corner.x);
        float maxY = screenCorners.Max(corner => corner.y);
        float minY = screenCorners.Min(corner => corner.y);

        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        return new Vector3[4] {topLeft, topRight, bottomRight, bottomLeft};
    }
}

public static class UnitExtentions
{
    public static Unit GetLowestHealth(this Unit[] units)
    {
        Unit result = units[0];
        for(int i = 1; i < units.Length; i++)
        {
            if(units[i].playerStatus.health < result.playerStatus.health && units[i].CanBeTargeted)
            {
                result = units[i];
            }
        }
        if(result.CanBeTargeted)
            return result;
        return null;
    }

    public static Unit GetHighestHealth(this Unit[] units)
    {
        Unit result = units[0];
        for(int i = 1; i < units.Length; i++)
        {
            if(units[i].playerStatus.health > result.playerStatus.health && units[i].CanBeTargeted)
            {
                result = units[i];
            }
        }
        if(result.CanBeTargeted)
            return result;
        return null;
    }
}


public static class GameObjectExtentions
{
    public static void DestroyThis(this GameObject gameObject)
    {
        MonoBehaviour.Destroy(gameObject);
    }
}