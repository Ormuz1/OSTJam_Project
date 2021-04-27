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
    [SerializeField] private RectTransform commandPrefab;
    [SerializeField] private RectTransform statusMenuTransform;
    [SerializeField] private AllyStatusGUI statusInfoPrefab;
    [SerializeField] int fontSize;

    [Header("Cursor Properties:")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private float cursorScale;
    [SerializeField] private PopupText popupTextPrefab;
    [SerializeField] private RadialTimer radialTimerPrefab;
    [SerializeField] private Vector2 radialTimerOffset;
    private Slider[] lifeBars;
    
    private Rect cursorRect;
    private bool isMenuDrawn = false;

    [HideInInspector] public int selectedAction;
    private bool drawCursorForTheFirstTime = true;

    private void Start() 
    {
        DrawLifebars();
    }
    private void OnGUI() 
    {
        if(isMenuDrawn)
        {
            CalculateCursorPosition();
            GUI.DrawTexture(cursorRect, cursorTexture);
        }
    }


    public void CalculateCursorPosition()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(commandMenuTransform);
        RectTransform actionRect = commandMenuTransform.GetChild(selectedAction) as RectTransform;
        Debug.Log(actionRect.pivot);
        Vector2 cursorPosition = new Vector2(
            actionRect.position.x - cursorTexture.width * cursorScale,
            (Screen.height - actionRect.position.y) - cursorTexture.height * cursorScale * .5f
            );
        cursorRect = new Rect(
            cursorPosition,
            new Vector2(cursorTexture.width, cursorTexture.height) * cursorScale
        );
    }

    public void DrawLifebars()
    {
        int childrenToDestroy = statusMenuTransform.childCount;
        for(int i = 0; i < childrenToDestroy; i++)
        {
            Destroy(statusMenuTransform.GetChild(i).gameObject);
        }
        int childrenToCreate = UnitManager.Instance.allies.Length;
        AllyStatusGUI[] statusInfos = new AllyStatusGUI[childrenToCreate];
        for(int i = 0; i < childrenToCreate; i++)
        {
            statusInfos[i] = Instantiate(statusInfoPrefab, statusMenuTransform) as AllyStatusGUI;
            UnitManager.Instance.allies[i].statusDisplay = statusInfos[i];
            statusInfos[i].UpdateInfo(UnitManager.Instance.allies[i].playerStatus);
        }
    }


    internal RadialTimer DrawRadialTimer(float duration, Unit unit)
    {
        if(!unit.meshBounds.Contains(transform.position))
            unit.meshBounds = unit.GetFullMeshBounds();
        RadialTimer radialTimer = Instantiate(radialTimerPrefab, transform) as RadialTimer;
        radialTimer.duration = duration;
        Vector3[] unitScreenCorners = unit.meshBounds.GetScreenCorners();
        radialTimer.GetComponent<RectTransform>().position = new Vector2(
            unitScreenCorners[1].x,
            unitScreenCorners[0].y
        ) + radialTimerOffset;
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
            RectTransform newCommand = Instantiate(commandPrefab, commandMenuTransform) as RectTransform;
            TextMeshProUGUI commandText = newCommand.GetComponentInChildren<TextMeshProUGUI>();
            commandText.text = commands[i].action.GetActionName();
            commandText.fontSize = fontSize;
        }
        selectedAction = 0;
        CalculateCursorPosition();
        isMenuDrawn = true;
    }


    public void CreatePopupText(Vector3 position, int value)
    {
        PopupText popupText = Instantiate(popupTextPrefab, position, Quaternion.identity, transform) as PopupText;

        popupText.Setup(value);
    }

    public void SetLifeBarMenuActive(bool state)
    {
        statusMenuTransform.gameObject.SetActive(state);
    }

    public void SetMenuActive(bool state)
    {
        commandMenuTransform.gameObject.SetActive(state);
        isMenuDrawn = state;
    }
}
