using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class MenuItem
{
    public string text;
    public UnityEvent onSelected;
}

[System.Serializable]
public class SubMenu
{
    public string name;
    public float itemSpacing;
    public Color textColor;
    public Color activeTextColor;
    public int fontSize;
    public int columns, rows;
    public MenuItem[] items;
    [HideInInspector] public int activeItemIndex = 0;
    [HideInInspector] public int MaxItemLength {
        get 
        {
            int[] textSizes = new int[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                textSizes[i] = items[i].text.Length;
            }
            int max = textSizes[0];
            for(int i = 1; i < items.Length; i++)
            {
                max = textSizes[i] > max ? textSizes[i] : max;
            }
            return max;
        }
    }
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SubMenu[] submenus;
    private SubMenu activeSubmenu;
    [SerializeField] private RectTransform menuTransform;
    [SerializeField] private RectTransform[] actions;
    [SerializeField] private Vector2 cursorOffset;
    [SerializeField] private float cursorScale;
    [SerializeField] private Texture2D cursorTexture;
    private int index = 0;
    private Rect cursorRect;
    public void Awake()
    {
        // FillMenu(submenus[0]);
        CalculateCursorPosition();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.S) && index < actions.Length -1)
        {
            index++;
            CalculateCursorPosition();
        }
        else if(Input.GetKeyDown(KeyCode.W) && index > 0)
        {
            index--;
            CalculateCursorPosition();
        }
    }
    private void OnGUI() 
    {
        GUI.DrawTexture(cursorRect, cursorTexture);
    }

    private void CalculateCursorPosition()
    {
        Vector2 actionPosition = actions[index].anchoredPosition;
        Rect actionRect = actions[index].rect;
        Vector2 cursorPosition = new Vector2(
            actionPosition.x - cursorTexture.width * cursorScale,
            Screen.height - (actionPosition.y + actionRect.height)
        );
        cursorRect = new Rect(
            cursorPosition,
            new Vector2(cursorTexture.width, cursorTexture.height) * cursorScale
        );
    }

    
    private void FillMenu(SubMenu submenu)
    {
        TextMeshProUGUI item = null;
        Vector3[] menuCorners = new Vector3[4];
        menuTransform.GetLocalCorners(menuCorners);
        for(int i = 0; i < submenu.items.Length; i++)
        {
            item = Instantiate(new GameObject(submenu.items[i].text), menuTransform).AddComponent<TextMeshProUGUI>();
            item.rectTransform.pivot = new Vector2(0, 1);
            item.rectTransform.localPosition = menuCorners[1] - Vector3.up * (item.rectTransform.rect.height * i);
            item.text = submenu.items[i].text;
            item.fontSize = submenu.fontSize;
            item.color = Color.black;
        }
        
    }
/* 
    public override void Awake() 
    {
        base.Awake();
        currentMenu = menus[0]; 
        for(int i = 0; i < currentMenu.items.Length; i++)
        {
            TextMeshProUGUI newText = GameObject.Create;
            newText.SetText(currentMenu.items[i].text);
            newText.transform.position = currentMenu.position *
            
        }   
    }
 */
}
