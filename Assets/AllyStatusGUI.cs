using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllyStatusGUI : MonoBehaviour
{
    private TextMeshProUGUI nameDisplay;
    private TextMeshProUGUI healthDisplay;
    [SerializeField] private Gradient healthColor;
    private void Awake() 
    {
        nameDisplay = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        healthDisplay = transform.GetChild(1).GetComponent<TextMeshProUGUI>();    
    }

    public void UpdateInfo(Unit.PlayerStatus status)
    {
        nameDisplay.text = status.unitName;
        healthDisplay.text = $"{status.health}/{status.maxHealth}";
        healthDisplay.color = healthColor.Evaluate((float)status.health / status.maxHealth);
    }
}
