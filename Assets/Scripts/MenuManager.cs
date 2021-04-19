using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : SingletonBase<MenuManager>
{
    private UnitActions[] commands;
    [SerializeField] private RectTransform menuTransform;
    [SerializeField] int fontSize;

    [Header("Cursor Properties:")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private float cursorScale;
    [SerializeField] private Vector2 cursorOffset;
    [SerializeField] private RectTransform lifeBarsHolder;
    [SerializeField] private Slider lifebarPrefab;
    private Slider[] lifeBars;
    
    private Rect cursorRect;
    private bool isMenuDrawn = false;
    [HideInInspector] public int selectedAction;
    private bool drawCursorForTheFirstTime = true;

    public void Start()
    {
        DrawLifebars();
    }
    
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


    public void UpdateLifeBar(Unit unit)
    {
        if(unit.healthBar)
        {
            unit.healthBar.value = (float)unit.health / unit.maxHealth;
            Debug.Log(unit.health);
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

    public void DrawLifebars()
    {
        int childrenToDestroy = lifeBarsHolder.childCount;
        for(int i = 0; i < childrenToDestroy; i++)
        {
            Destroy(lifeBarsHolder.GetChild(i).gameObject);
        }
        int childrenToCreate = UnitManager.Instance.allies.Length;
        lifeBars = new Slider[childrenToCreate];
        for(int i = 0; i < childrenToCreate; i++)
        {
            lifeBars[i] = Instantiate(lifebarPrefab, lifeBarsHolder) as Slider;
            UnitManager.Instance.allies[i].healthBar = lifeBars[i];
            lifeBars[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = UnitManager.Instance.allies[i].unitName;
        }
    }
    public void FillMenu(Unit unit)
    {
        UnitCommand[] commands = unit.commands;
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
