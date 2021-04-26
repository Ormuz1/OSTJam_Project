using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public void Lose()
    {
        Debug.Log("You lose");
        Debug.Break();
    }
}
