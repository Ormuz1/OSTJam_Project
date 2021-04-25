using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour 
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float timeToDisapear = 1f;
    [SerializeField] private float scaleIncreaseAmount = 1f;
    [SerializeField] private float scaleDecreaseAmount = 1f;
    [SerializeField, Range(0,1)] private float scaleTime = 0.5f;
    [SerializeField] private Color damageColor = Color.green;
    [SerializeField] private Color healColor = Color.red;
    [SerializeField] private Vector2 textMovement = Vector2.one * 60f;

    private RectTransform rectTransform;
    private TextMeshProUGUI textMesh;
    private Color textColor;
    private Vector2 randomDirection;
    private float timer = 0;
    private float startAlpha;
    private void Awake() 
    {
        textMesh = transform.GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

 
    public void Setup(int damageAmount) {
        randomDirection = Random.insideUnitCircle.normalized;
        string damageString = Mathf.Abs(damageAmount).ToString();
        if (damageAmount < 0) {
            textMesh.SetText("+" + damageString);
            textColor = healColor;
        } else 
        {
            textMesh.SetText("-" + damageString);
            textColor = damageColor;
        }
        textMesh.color = textColor;
        startAlpha = textMesh.color.a;
    }

 
    private void Update() {
        rectTransform.position += (Vector3)(textMovement * randomDirection * Time.deltaTime);

        if (timer < duration * scaleTime) {
            // First half of the popup lifetime
            transform.localScale += Vector3.one * scaleIncreaseAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            transform.localScale -= Vector3.one * scaleDecreaseAmount * Time.deltaTime;
        }
        if (timer > duration) 
        {
            textColor.a = Mathf.Lerp(startAlpha, 0, (timer - duration) / timeToDisapear);
            textMesh.color = textColor;
            if (timer > duration + timeToDisapear) {
                Destroy(gameObject);
            }
        }
        timer += Time.deltaTime;
    }

}