using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : SingletonBase<BattleManager>
{
    [SerializeField] private Vector3 alliesRow;
    [SerializeField] private Vector3 enemyFrontRow;
    public GameObject[] allies;
    private GameObject[] allyInstances;
    public GameObject[] enemies;
    private GameObject[] enemyInstances;
    public Character activeCharacter;
    public Character attackTarget;
    [SerializeField] private bool showCharactersInEditMode;
    public override void Awake()
    {
        base.Awake();
        InstantiateCharacters();
        activeCharacter = allyInstances[0].GetComponent<Character>();
        attackTarget = enemyInstances[0].GetComponent<Character>();
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && activeCharacter.state == CharacterStates.Idle)
        {
            activeCharacter.StartCoroutine(activeCharacter.Attack(attackTarget));
        }
    }

    private void InstantiateCharacters()
    {
        if(allyInstances == null)
        {
            allyInstances = new GameObject[allies.Length];
            for (int i = 0; i < allies.Length; i++)
            {
                GameObject allyInstance = Instantiate(allies[i], alliesRow, Quaternion.identity);
                allyInstance.transform.parent = transform;
                allyInstances[i] = allyInstance;
            }
        }
        
        enemyInstances = new GameObject[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemyInstance = Instantiate(enemies[i], enemyFrontRow, Quaternion.identity);
            enemyInstance.transform.parent = transform;
            enemyInstances[i] = enemyInstance;
        }
    }

    private void DestroyAll(GameObject[] instances)
    {
        for(int i = 0; i < instances.Length; i++)
        {
            DestroyImmediate(instances[i]);
        }
    }
}
