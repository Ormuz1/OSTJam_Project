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
    [Serializable]
    public class PlayerStatus
    {
        public string unitName;
        public int maxHealth;
        [HideInInspector] public int health;
    }

    public PlayerStatus playerStatus;
    public int damage;
    
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
    [HideInInspector] public float timeUntilCanChangeAnimation = 0f;


    protected virtual void Awake() 
    {
        playerStatus.health = playerStatus.maxHealth;
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
            timeUntilCanChangeAnimation = Time.time + clip.length;
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


    public virtual void OnHealthChanged(int amount)
    {
        if(amount > 0)
        {
            if(isGuarding) amount /= 2;
            if(playerStatus.health <= 0) OnDeath();
            else UnitManager.Instance.unitSfxPlayer.PlayRandom(hurtSoundEffects);
            PlayAnimation(hurtAnimation);
        }

        playerStatus.health = Mathf.Clamp(playerStatus.health - amount, 0,playerStatus.maxHealth);
        MenuManager.Instance.CreatePopupText(Camera.main.WorldToScreenPoint(transform.position), amount);
    }


    protected virtual void OnDeath()
    {
        UnitManager.Instance.unitSfxPlayer.PlayRandom(deathSoundEffect);
        if(currentRadialTimer)
            Destroy(currentRadialTimer.gameObject);
    }
}
