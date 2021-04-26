using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Unit
{
    private const float KO_DURATION = 7f;
    public UnitCommand[] commands;
    public const float ATTACK_ANIMATION_WINDUP_TIME = 0.284f;
    [HideInInspector] public AllyStatusGUI statusDisplay;
    protected override void OnDeath()
    {
        base.OnDeath();
        StartCoroutine(KnockedOut());
    }

    public override void OnHealthChanged(int amount)
    {
        base.OnHealthChanged(amount);
        statusDisplay.UpdateInfo(playerStatus);
    }

    private IEnumerator KnockedOut()
    {
        if(state == UnitStates.CannotAction)
        {
            state = UnitStates.InterruptAction;
        }
        yield return new WaitUntil(() => state == UnitStates.CanAction);
        state = UnitStates.KnockedOut;
        PlayAnimation(deathAnimation);
        yield return new WaitForSeconds(deathAnimation.length / 2);
        animator.SetFloat("AnimationSpeed", 0);
        yield return new WaitForSeconds(KO_DURATION - deathAnimation.length);
        animator.SetFloat("AnimationSpeed", 1);
        StartCoroutine(ResetAnimation(deathAnimation.length / 2));
        MenuManager.Instance.DrawRadialTimer(KO_DURATION, this);
        playerStatus.health = playerStatus.maxHealth;
        lifeBar.value = (float)playerStatus.health / playerStatus.maxHealth;
        state = UnitStates.CanAction;
    }
}
