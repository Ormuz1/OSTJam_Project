using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Unit
{
    private const float KO_DURATION = 7f;
    public UnitCommand[] commands;
    public const float ATTACK_ANIMATION_WINDUP_TIME = 0.53f;
    [HideInInspector] public AllyStatusGUI statusDisplay;


    public override void OnHealthChanged(int amount)
    {
        base.OnHealthChanged(amount);
        statusDisplay.UpdateInfo(playerStatus);
    }


    protected override void OnDeath()
    {
        base.OnDeath();
        StartCoroutine(KnockedOut());
    }


    private IEnumerator KnockedOut()
    {
        if(state == UnitStates.CannotAction)
        {
            state = UnitStates.InterruptAction;
        }
        else if(state == UnitStates.KnockedOut)
        {
            yield break;
        }
        yield return new WaitUntil(() => state == UnitStates.CanAction);
        state = UnitStates.KnockedOut;
        if(CommandManager.Instance.activeUnit == this)
        {
            CommandManager.Instance.SetActiveUnit();
        }
        PlayAnimation(deathAnimation, deathAnimation.length);
        StopCoroutine(animationReset);
        MenuManager.Instance.DrawRadialTimer(KO_DURATION, this);
        yield return new WaitForSeconds(deathAnimation.length / 2);
        animator.SetFloat("AnimationSpeed", 0);
        yield return new WaitForSeconds(KO_DURATION - deathAnimation.length);
        StartCoroutine(ResetAnimation(deathAnimation.length / 2));
        animator.SetFloat("AnimationSpeed", 1);
        yield return new WaitForSeconds(deathAnimation.length / 2);
        playerStatus.health = playerStatus.maxHealth;
        statusDisplay.UpdateInfo(playerStatus);
        state = UnitStates.CanAction;
    }
}
