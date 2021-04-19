using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UnitStates { CanAction, CannotAction, InterruptAction}
public enum UnitActions { Attack, Heal, StopHealing}

[System.Serializable]
public class UnitCommand
{
    public UnitActions action;
    public float duration;
}

public class Unit : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int health;

    public int damage;
    public string unitName;
    [HideInInspector] public UnitStates state = UnitStates.CanAction;
    public UnitCommand[] commands;
    private const float attackTargetDistance = -1.2f;
    [HideInInspector] public UnityEngine.UI.Slider healthBar;

    protected virtual void Awake() 
    {
        health = maxHealth;
    }

    public void ExecuteAction(UnitCommand command, Unit commandTarget = null)
    {
        Debug.Log("Executing Action");
        switch(command.action)
        {
            case UnitActions.Attack:
                StartCoroutine(CommandCoroutines.Attack(this, commandTarget, command.duration));               
                break;
            case UnitActions.Heal:
                StartCoroutine(CommandCoroutines.StartHealing(this, commandTarget));
                break;
            case UnitActions.StopHealing:
                state = UnitStates.InterruptAction;
                break; 
        }
    }
}
