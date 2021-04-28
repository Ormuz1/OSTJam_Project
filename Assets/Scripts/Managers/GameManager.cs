using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameManager : SingletonBase<GameManager>
{
    public SceneReference[] levels;
    [HideInInspector] public int currentLevel;
    public SceneReference gameOverScene;
    
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }


    public void Lose()
    {
        SceneManager.LoadScene(gameOverScene.ScenePath);
        StartCoroutine(ResetLevel());
    }

    private IEnumerator ResetLevel()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(levels[currentLevel].ScenePath);
    }
    internal void GotoNextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene(levels[currentLevel].ScenePath);
    }
}