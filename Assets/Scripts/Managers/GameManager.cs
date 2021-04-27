using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameManager : SingletonBase<GameManager>
{
    public SceneReference[] levels;
    [HideInInspector] public int currentLevel;
    
    
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }
    
    
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            currentLevel++;
            SceneManager.LoadScene(levels[currentLevel]);
        }    
    }


    public void Lose()
    {
        Debug.Log("You lose");
        Debug.Break();
        CommandManager.Instance.DisableCommandMenu();
    }


    internal void GotoNextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(levels[currentLevel]);
    }
}