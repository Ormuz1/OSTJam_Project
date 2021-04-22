using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Reflection;
public enum UnitStates { CanAction, CannotAction, InterruptAction}
public enum UnitActions { Attack, Heal, Stop}

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
    public int maxHealth;
    [HideInInspector] public int health;
    public int damage;
    public string unitName;
    
    [HideInInspector] public UnitStates state;
    private const float attackTargetDistance = -1.2f;
    [HideInInspector] public UnityEngine.UI.Slider lifeBar;
    [HideInInspector] public Bounds meshBounds;
    [HideInInspector] public RadialTimer currentRadialTimer;
    protected virtual void Awake() 
    {
        health = maxHealth;
        meshBounds = GetComponent<MeshRenderer>().bounds; 
    }

    public void ExecuteAction(UnitCommand command, Unit commandTarget = null)
    {
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
        }
    }

    public void OnHealthChanged(int amount)
    {
        MenuManager.Instance.CreatePopupText(Camera.main.WorldToScreenPoint(transform.position), amount);
        if(lifeBar)
        {
            lifeBar.value = (float)health / maxHealth;
        }
        if(health < 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        Destroy(currentRadialTimer.gameObject);
    }
}
