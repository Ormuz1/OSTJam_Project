using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UnitStates { Idle, Busy, Interrupt}
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
    [HideInInspector] public UnitStates state = UnitStates.Idle;
    public UnitCommand[] commands;
    private const float attackTargetDistance = -1.2f;
    private const float timeBetweenHeals = 0.8f;
    [HideInInspector] public UnityEngine.UI.Slider healthBar;

    protected virtual void Awake() 
    {
        health = maxHealth;
    }

    public IEnumerator Attack(Unit attackTarget, float duration)
    {
        float moveTime = duration * 0.9f * 0.5f;
        Vector3 startPosition = transform.position, velocity = Vector3.zero;
        state = UnitStates.Busy;
        yield return MoveToPosition(attackTarget.transform.position, moveTime);
        attackTarget.health -= damage;
        MenuManager.Instance.UpdateLifeBar(attackTarget);
        yield return new WaitForSeconds(duration * 0.1f);
        yield return MoveToPosition(startPosition, moveTime);
        state = UnitStates.Idle;
    }

    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        Vector3 startPosition = transform.position;
        float t;
        for(float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            t = timer / duration;
            t = t*t*t * (t * (6f*t - 15f) + 10f);
            transform.position = Vector3.Lerp(startPosition, target, t);
            yield return null;
        }
    }
    
    public IEnumerator StartHealing(Unit target)
    {
        yield return null;
        state = UnitStates.Idle;
        WaitForSeconds waitTime = new WaitForSeconds(timeBetweenHeals);
        UnitCommand[] commandCache = commands;
        UnitCommand stopHealingCommand = new UnitCommand
        {
            action = UnitActions.StopHealing
        };
        commands = new UnitCommand[] {stopHealingCommand};
        while(state != UnitStates.Interrupt && target.health < target.maxHealth)
        {
            Debug.Log("Healing");
            target.health = Mathf.Min(target.health + 10, target.maxHealth);
            MenuManager.Instance.UpdateLifeBar(target);
            yield return waitTime;
        }
        state = UnitStates.Busy;
        yield return null;
        state = UnitStates.Idle;
    }

    public void ExecuteAction(UnitCommand command, Unit commandTarget = null)
    {
        Debug.Log("Executing Action");
        state = UnitStates.Idle;
        switch(command.action)
        {
            case UnitActions.Attack:
                StartCoroutine(Attack(commandTarget, command.duration));               
                break;
            case UnitActions.Heal:
                StartCoroutine(StartHealing(commandTarget));
                break;
            case UnitActions.StopHealing:
                Debug.Log("Interrupting");
                state = UnitStates.Interrupt;
                break; 
        }
    }
}
