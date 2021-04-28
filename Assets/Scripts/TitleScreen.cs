using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TitleScreen : MonoBehaviour
{
    [SerializeField] private SceneReference firstLevel;

    void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene(firstLevel.ScenePath);
        }
    }
}
