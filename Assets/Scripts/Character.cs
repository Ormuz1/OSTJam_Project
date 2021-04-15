using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStates { Idle, Attacking}

[System.Serializable]
public struct CharacterData
{
    public int health;
    public int damage;
    public float slideSpeed;
}

public class Character : MonoBehaviour
{
    public CharacterStates state;
    public CharacterData characterData;
    private const float reachedDestination = .1f;
    public IEnumerator Attack(Character attackTarget)
    {
        Vector3 startPosition = transform.position;
        state = CharacterStates.Attacking;
        yield return SlideToPosition(attackTarget.transform.position);
        attackTarget.characterData.health -= characterData.damage;
        yield return new WaitForSeconds(1f);
        yield return SlideToPosition(startPosition);
        state = CharacterStates.Idle;
    }

    private IEnumerator SlideToPosition(Vector3 targetPosition)
    {
        while(Vector3.Distance(transform.position, targetPosition) > reachedDestination)
        {
            transform.position += (targetPosition - transform.position) * characterData.slideSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
