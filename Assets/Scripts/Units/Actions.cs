using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitActions { Attack, Heal, Defend, Stop}

public static class CommandCoroutines
{
    private const float timeBetweenHeals = 0.8f;
    private const float attackMoveDistance = 1f;


    public static IEnumerator StartHealing(Unit origin, Unit target)
    {
        bool isAlly = origin.GetType() == typeof(Ally);
        Ally originAsAlly = null;
        UnitCommand[] commandCache = null;
        if(isAlly)
        {
            originAsAlly = origin as Ally;
            commandCache = originAsAlly.commands;
            UnitCommand stopHealingCommand = new UnitCommand(UnitActions.Stop, 0);
            originAsAlly.commands = new UnitCommand[] {stopHealingCommand};
            origin.state = UnitStates.CannotAction;
            yield return null;
        }
        origin.state = UnitStates.CanAction;
        for(float timer = 0; true; timer += Time.deltaTime)
        {
            if(origin.state == UnitStates.InterruptAction)
                break;

            if(!(timer < timeBetweenHeals))
            {
                timer = 0;
                target.health = Mathf.Min(target.health + 10, target.maxHealth);
                target.OnHealthChanged(-10);
            }
            yield return null;
        }
        if(isAlly)
        {
            originAsAlly.commands = commandCache;
            MenuManager.Instance.FillMenu(originAsAlly);
        }
        else
        {
            origin.state = UnitStates.CanAction;
        }
    }

    public static IEnumerator Defend(Unit origin, float duration)
    {
        bool isAlly = origin.GetType() == typeof(Ally);
        Ally originAsAlly = null;
        UnitCommand[] commandCache = null;
        if(isAlly)
        {
            originAsAlly = origin as Ally;
            commandCache = originAsAlly.commands;
            UnitCommand stopCommand = new UnitCommand(UnitActions.Stop, 0);
            originAsAlly.commands = new UnitCommand[] {stopCommand};
            origin.state = UnitStates.CannotAction;
            yield return null;
        }
        origin.state = UnitStates.CanAction;
        origin.isGuarding = true;
        for(float timer = 0; true; timer += Time.deltaTime)
        {
            if(origin.state == UnitStates.InterruptAction || (!isAlly && timer > duration))
                break;
            yield return null;
        }
        origin.isGuarding = false;
        if(isAlly)
        {
            originAsAlly.commands = commandCache;
            MenuManager.Instance.FillMenu(originAsAlly);
        }
        else
        {
            origin.state = UnitStates.CanAction;
        }
    }
        
    public static IEnumerator Attack(Unit origin, Unit target, float duration)
    {
        float moveTime = duration * 0.9f * 0.5f;
        Vector3 startPosition = origin.transform.position, velocity = Vector3.zero;
        Vector3 forwardMovement = Vector3.forward * attackMoveDistance * Mathf.Sign(target.transform.position.z - origin.transform.position.z);
        origin.state = UnitStates.CannotAction;
        yield return MoveToPosition(origin, origin.transform.position + forwardMovement, moveTime);
        target.health -= origin.damage;
        target.OnHealthChanged(origin.damage);
        yield return new WaitForSeconds(duration * 0.1f);
        yield return MoveToPosition(origin, startPosition, moveTime);
        origin.state = UnitStates.CanAction;
    }


    public static IEnumerator MoveToPosition(Unit origin, Vector3 target, float duration)
    {
        Vector3 startPosition = origin.transform.position;
        float t;
        for(float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            t = timer / duration;
            t = t*t*t * (t * (6f*t - 15f) + 10f);
            origin.transform.position = Vector3.Lerp(startPosition, target, t);
            yield return null;
        }
    }
}
