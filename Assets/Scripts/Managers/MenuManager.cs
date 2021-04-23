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
    [SerializeField] private RectTransform commandMenuTransform;
    [SerializeField] int fontSize;

    [Header("Cursor Properties:")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private float cursorScale;
    [SerializeField] private Vector2 cursorOffset;
    [SerializeField] private RectTransform lifeBarsHolder;
    [SerializeField] private Slider lifebarPrefab;
    [SerializeField] private PopupText popupTextPrefab;
    private Slider[] lifeBars;
    
    private Rect cursorRect;
    private bool isMenuDrawn = false;
    [SerializeField] private RadialTimer radialTimerPrefab;

    [HideInInspector] public int selectedAction;
    private bool drawCursorForTheFirstTime = true;

    private void Start() 
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


    public void CalculateCursorPosition()
    {
        List<RectTransform> actions = new List<RectTransform>();
        commandMenuTransform.GetComponentsInChildren<RectTransform>(actions);
        actions.Remove(commandMenuTransform);
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
            UnitManager.Instance.allies[i].lifeBar = lifeBars[i];
            lifeBars[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = UnitManager.Instance.allies[i].unitName;
        }
    }


    internal RadialTimer DrawRadialTimer(float duration, Unit unit)
    {
        RadialTimer radialTimer = Instantiate(radialTimerPrefab, transform) as RadialTimer;
        radialTimer.duration = duration;
        Vector3[] unitScreenCorners = unit.meshBounds.GetScreenCorners();
        radialTimer.GetComponent<RectTransform>().position = unitScreenCorners[1] + unit.meshBounds.size * 1.5f;
        radialTimer.associatedUnit = unit;
        return radialTimer;
    }


    public void FillMenu(Ally unit)
    {
        UnitCommand[] commands = unit.commands;
        commandMenuTransform.gameObject.SetActive(true);
        int childrenToDestroy  = commandMenuTransform.childCount;
        for(int i = 0; i < childrenToDestroy; i++)
        {
            Destroy(commandMenuTransform.GetChild(i).gameObject);
        }
        for(int i = 0; i < commands.Length; i++)
        {
            TextMeshProUGUI item;
            GameObject itemObject = new GameObject(commands[i].action.GetActionName());
            itemObject.transform.parent = commandMenuTransform; 
            item = itemObject.AddComponent<TextMeshProUGUI>();
            item.rectTransform.pivot = new Vector2(0, 1);
            item.text = commands[i].action.GetActionName();
            item.fontSize = fontSize;
            item.color = Color.white;
        }
        selectedAction = 0;
        CalculateCursorPosition();
        isMenuDrawn = true;
    }


    public void CreatePopupText(Vector3 position, int value)
    {
        PopupText popupText = Instantiate(popupTextPrefab, position, Quaternion.identity, transform) as PopupText;
        popupText.transform.SetSiblingIndex(0);     // Draw it in front of other UI elements

        popupText.Setup(value);
    }

    public void SetLifeBarMenuActive(bool state)
    {
        lifeBarsHolder.parent.gameObject.SetActive(state);
    }

    public void SetMenuActive(bool state)
    {
        commandMenuTransform.gameObject.SetActive(state);
        isMenuDrawn = state;
    }
}
