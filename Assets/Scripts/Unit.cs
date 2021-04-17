using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UnitStates { Idle, Busy}
public enum UnitActions { Attack, Wait, Heal}

[System.Serializable]
public class UnitCommand
{
    public UnitActions action;
    public float duration;
}

[System.Serializable]
public struct UnitData
{
    public int health;
    public int damage;
}

public class Unit : MonoBehaviour
{
    public UnitStates state;
    public UnitData unitData;
    public UnitCommand[] actions;
    private const float attackTargetDistance = -1.2f;
    public IEnumerator Attack(Unit attackTarget, float duration)
    {
        float moveTime = duration * 0.9f * 0.5f;
        Vector3 startPosition = transform.position, velocity = Vector3.zero;
        state = UnitStates.Busy;
        yield return MoveToObject(attackTarget.transform, moveTime);
        attackTarget.unitData.health -= unitData.damage;
        yield return new WaitForSeconds(duration * 0.1f);
        yield return MoveToPosition(startPosition, moveTime);
        state = UnitStates.Idle;
    }

    private IEnumerator MoveToObject(Transform target, float duration)
    {
        Vector3 startPosition = transform.position; 
        Vector2 targetDirection = new Vector2(
            target.position.x - transform.position.x,
            target.position.z - transform.position.z
            ).normalized;
        Vector3 targetPosition = new Vector3(
            target.position.x + attackTargetDistance * targetDirection.x,
            transform.position.y,
            target.position.z + attackTargetDistance * targetDirection.y
        );
        Vector2 thisPositionXZ, targetPositionXZ;
        float t;
        for(float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            thisPositionXZ = new Vector2(transform.position.x, transform.position.z);
            targetPositionXZ = new Vector2(target.position.z, target.position.z);
            if(Vector2.Distance(thisPositionXZ, targetPositionXZ) < attackTargetDistance)
            {
                yield break;
            }
            t = timer / duration;
            t = t*t*t * (t * (6f*t - 15f) + 10f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
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
    public IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        state = UnitStates.Idle;
    }

    public void ExecuteAction(UnitCommand command, Unit commandTarget = null)
    {
        state = UnitStates.Busy;
        switch(command.action)
        {
            case UnitActions.Attack:
                StartCoroutine(Attack(commandTarget, command.duration));               
                break;
                
            default:
                StartCoroutine(Wait(command.duration));
                break;
        }
    }
}
