using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCoroutines
{
    private const float timeBetweenHeals = 0.8f;
    private const float attackMoveDistance = 1f;


    public static IEnumerator StartHealing(Unit origin, Unit target)
    {
        UnitCommand[] commandCache = origin.commands;
        UnitCommand stopHealingCommand = new UnitCommand
        {
            action = UnitActions.StopHealing
        };
        origin.commands = new UnitCommand[] {stopHealingCommand};
        origin.state = UnitStates.CannotAction;
        yield return null;
        origin.state = UnitStates.CanAction;
        for(float timer = 0; true; timer += Time.deltaTime)
        {
            if(origin.state == UnitStates.InterruptAction)
                break;

            if(!(timer < timeBetweenHeals))
            {
                timer = 0;
                target.health = Mathf.Min(target.health + 10, target.maxHealth);
                MenuManager.Instance.UpdateLifeBar(target);
            }
            yield return null;
        }
        origin.commands = commandCache;
        MenuManager.Instance.FillMenu(origin);
    }


    public static IEnumerator Attack(Unit attackOrigin, Unit attackTarget, float duration)
    {
        float moveTime = duration * 0.9f * 0.5f;
        Vector3 startPosition = attackOrigin.transform.position, velocity = Vector3.zero;
        Vector3 forwardMovement = Vector3.forward * attackMoveDistance * Mathf.Sign(attackTarget.transform.position.z - attackOrigin.transform.position.z);
        attackOrigin.state = UnitStates.CannotAction;
        yield return MoveToPosition(attackOrigin, attackOrigin.transform.position + forwardMovement, moveTime);
        attackTarget.health -= attackOrigin.damage;
        MenuManager.Instance.UpdateLifeBar(attackTarget);
        yield return new WaitForSeconds(duration * 0.1f);
        yield return MoveToPosition(attackOrigin, startPosition, moveTime);
       attackOrigin.state = UnitStates.CanAction;
    }


    private static IEnumerator MoveToPosition(Unit origin, Vector3 target, float duration)
    {
        Vector3 startPosition = origin.transform.position;
        float t;
        for(float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            t = timer / duration;
            t = t*t*t * (t * (6f*t - 15f) + 10f);
            origin.transform.localPosition = Vector3.Lerp(startPosition, target, t);
            yield return null;
        }
    }
}
