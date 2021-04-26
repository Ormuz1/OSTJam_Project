using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Reflection;
public enum UnitStates { CanAction, CannotAction, InterruptAction, KnockedOut}

[System.Serializable]
public class UnitCommand
{
    public UnitActions action;
    public float duration;

    public UnitCommand(UnitActions action, float duration)
    {
        this.action = action;
        this.duration = duration;
    }
}

public class Unit : MonoBehaviour
{
    private const float attackTargetDistance = -1.2f;
    public bool CanBeTargeted { get => state != UnitStates.KnockedOut;}
    [HideInInspector] public UnityEngine.UI.Slider lifeBar;
    [HideInInspector] public Bounds meshBounds;
    [HideInInspector] public RadialTimer currentRadialTimer;
    [HideInInspector] public bool isGuarding = false;
    [HideInInspector] public Animator animator;
    
    public int maxHealth;
    [HideInInspector] public int health;
    public int damage;
    public string unitName;
    
    public UnitStates state;
    [Header("Animations")]
    public AnimationClip attackAnimation = null;
    public AnimationClip runAnimation = null;
    public AnimationClip idleAnimation = null;
    public AnimationClip deathAnimation = null;
    public AnimationClip hurtAnimation = null;
    [Header("Sound Effects")]
    public SoundEffect[] executingActionSoundEffects;
    public SoundEffect[] attackSoundEffects;
    public SoundEffect[] hurtSoundEffects;
    public SoundEffect[] deathSoundEffect;
    public Vector3 unitCursorOffset;
    private float timeUntilCanChangeAnimation = 0f;
    protected virtual void Awake() 
    {
        health = maxHealth;
        var renderers = GetComponentsInChildren<Renderer>();
        meshBounds = renderers[0].bounds;
        foreach(Renderer childRenderer in renderers)
        {
            meshBounds.Encapsulate(childRenderer.bounds);
        }
        animator = GetComponentInChildren<Animator>();
    }

    public void ExecuteAction(UnitCommand command, Unit commandTarget = null)
    {
        UnitManager.Instance.unitSfxPlayer.PlayRandom(executingActionSoundEffects);
        state = UnitStates.CannotAction;
        switch(command.action)
        {
            case UnitActions.Attack:
                StartCoroutine(CommandCoroutines.Attack(this, commandTarget, command.duration));               
                break;
            case UnitActions.Heal:
                StartCoroutine(CommandCoroutines.StartHealing(this, commandTarget));
                break;
            case UnitActions.Stop:
                state = UnitStates.InterruptAction;
                break;
            case UnitActions.Defend:
                StartCoroutine(CommandCoroutines.Defend(this, command.duration));
                break;
            case UnitActions.TimeSpeedUp:
                TimerManager.Instance.countdownSpeedMultiplier += .25f;
                break;
        }
    }
    
    public void PlayAnimation(AnimationClip clip, float speed = 1f)
    {
        if(Time.time > timeUntilCanChangeAnimation && animator && clip)
        {
            animator.SetFloat("AnimationSpeed", speed);
            animator.Play(clip.name);
            StopCoroutine("ResetAnimation");
            StartCoroutine(ResetAnimation(clip.length));
        }
    }
    public IEnumerator ResetAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(idleAnimation.name);
        animator.SetFloat("AnimationSpeed", 1);
    }
    public void OnHealthChanged(int amount)
    {
        if(amount > 0)
        {
            if(isGuarding) amount /= 2;
            if(health <= 0) OnDeath();
            else UnitManager.Instance.unitSfxPlayer.PlayRandom(hurtSoundEffects);
            PlayAnimation(hurtAnimation);
        }

        health = Mathf.Clamp(health - amount, 0, maxHealth);
        MenuManager.Instance.CreatePopupText(Camera.main.WorldToScreenPoint(transform.position), amount);
        if(lifeBar)
        {
            lifeBar.value = (float)health / maxHealth;
        }
    }

    protected virtual void OnDeath()
    {
        UnitManager.Instance.unitSfxPlayer.PlayRandom(deathSoundEffect);
        if(currentRadialTimer)
            Destroy(currentRadialTimer.gameObject);
    }

}
