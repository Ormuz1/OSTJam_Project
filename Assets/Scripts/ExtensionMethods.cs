using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class Extentions
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
        switch(action)
        {
            case UnitActions.Heal:
                return UnitManager.Instance.allies.Concat(UnitManager.Instance.enemies).ToArray();
            case UnitActions.Attack:
                return UnitManager.Instance.enemies.Concat(UnitManager.Instance.allies).ToArray();
            default:
                return null;
        }
    }
}