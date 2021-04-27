using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitActions { Attack, Heal, Defend, Stop, TimeSpeedUp, StrongAttack}

public static class CommandCoroutines
{
    private const float timeBetweenHeals = 0.8f;
    private const float attackMoveDistance = 1f;
    private static readonly Range ATTACK_RANDOM_MODIFIER_RANGE = new Range(-3, 5);

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
            if(origin.state == UnitStates.InterruptAction || target == null)
                break;

            if(!(timer < timeBetweenHeals))
            {
                timer = 0;
                target.OnHealthChanged(-10);
            }
            yield return null;
        }
        if(isAlly)
        {
            originAsAlly.commands = commandCache;
            MenuManager.Instance.FillMenu(originAsAlly);
        }
        origin.state = UnitStates.CanAction;
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
        origin.state = UnitStates.CanAction;
    }
        

    public static IEnumerator Attack(Unit origin, Unit target, float duration)
    {
        origin.state = UnitStates.CannotAction;
        float windupTime = origin.GetType() == typeof(Ally) ? Ally.ATTACK_ANIMATION_WINDUP_TIME : SimpleEnemy.ATTACK_ANIMATION_WINDUP_TIME;
        windupTime *= origin.attackAnimation.length / duration;
        if(Time.time < origin.timeUntilCanChangeAnimation)
            yield return new WaitForSeconds(origin.timeUntilCanChangeAnimation - Time.time);
        origin.PlayAnimation(origin.attackAnimation, duration, origin.attackAnimation.length / duration);
        yield return new WaitForSeconds(windupTime);
        UnitManager.Instance.unitSfxPlayer.PlayRandom(origin.attackSoundEffects);
        target.OnHealthChanged(origin.damage + ATTACK_RANDOM_MODIFIER_RANGE.Random());
        yield return new WaitForSeconds(duration - windupTime);
        origin.state = UnitStates.CanAction;
    }

    public static IEnumerator StrongAttack(Unit origin, Unit target, float duration)
    {
        origin.state = UnitStates.CanAction;
        float windupTime = Ally.ATTACK_ANIMATION_WINDUP_TIME;
        MenuManager.Instance.DrawRadialTimer(6f, origin);
        yield return new WaitForSeconds(6f);
        origin.PlayAnimation(origin.attackAnimation, origin.attackAnimation.length);
        yield return new WaitForSeconds(windupTime);
        UnitManager.Instance.unitSfxPlayer.Play(origin.strongAttackSoundEffect);
        target.OnHealthChanged(origin.damage * 2);
        yield return new WaitForSeconds(origin.attackAnimation.length - windupTime);
        origin.state = UnitStates.CanAction;
    }

    public static IEnumerator MoveToPosition(Unit origin, Vector3 target, float duration)
    {
        origin.PlayAnimation(origin.runAnimation, origin.runAnimation.length);
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
