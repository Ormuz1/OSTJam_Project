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
}