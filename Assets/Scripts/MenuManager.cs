using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    private UnitActions[] commands;
    [SerializeField] private RectTransform menuTransform;
    [SerializeField] int fontSize;

    [Header("Cursor Properties:")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private float cursorScale;
    [SerializeField] private Vector2 cursorOffset;
    private Rect cursorRect;
    private bool isMenuDrawn = false;
    [HideInInspector] public int selectedAction;
    private bool drawCursorForTheFirstTime = true;
    private void OnGUI() 
    {
        if(drawCursorForTheFirstTime)
        {
            if(Event.current.type == EventType.Repaint)
            {
                CalculateCursorPosition();
                drawCursorForTheFirstTime = false;
            }
        }
        if(isMenuDrawn)
        {
            GUI.DrawTexture(cursorRect, cursorTexture);
        }
    }


    public void CalculateCursorPosition()
    {
        List<RectTransform> actions = new List<RectTransform>();
        menuTransform.GetComponentsInChildren<RectTransform>(actions);
        actions.Remove(menuTransform);
        RectTransform actionRect = actions[selectedAction];
        Vector2 cursorPosition = new Vector2(
            actionRect.position.x - cursorTexture.width * cursorScale,
            (Screen.height - actionRect.position.y) + cursorTexture.height * .5f
            ) + cursorOffset;
        cursorRect = new Rect(
            cursorPosition,
            new Vector2(cursorTexture.width, cursorTexture.height) * cursorScale
        );
    }

    
    public void FillMenu(UnitCommand[] commands)
    {
        menuTransform.gameObject.SetActive(true);
        int childrenToDestroy  = menuTransform.childCount;
        for(int i = 0; i < childrenToDestroy; i++)
        {
            Destroy(menuTransform.GetChild(i).gameObject);
        }
        for(int i = 0; i < commands.Length; i++)
        {
            TextMeshProUGUI item;
            GameObject itemObject = new GameObject(commands[i].action.GetActionName());
            itemObject.transform.parent = menuTransform; 
            item = itemObject.AddComponent<TextMeshProUGUI>();
            item.rectTransform.pivot = new Vector2(0, 1);
            item.text = commands[i].action.GetActionName();
            item.fontSize = fontSize;
            item.color = Color.black;
        }
        selectedAction = 0;
        CalculateCursorPosition();
        isMenuDrawn = true;
    }


    public void SetMenuActive(bool state)
    {
        menuTransform.gameObject.SetActive(state);
        isMenuDrawn = state;
    }
}
