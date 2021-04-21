using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour 
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float timeToDisapear = 1f;
    [SerializeField] private Color damageColor;
    [SerializeField] private Color healColor;
    [SerializeField] private Vector2 textMovement = new Vector3(.7f, 1) * 60f;

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer;
    private float startAlpha;
    private RectTransform thisTransform;
 
    private void Awake() 
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        thisTransform = GetComponent<RectTransform>();
    }

 
    public void Setup(int damageAmount) {
        textMesh.SetText(damageAmount.ToString());
        if (damageAmount < 0) {
            textMesh.color = healColor;
        } else {
            textMesh.color = damageColor;
        }
        timer = duration;
        startAlpha = textMesh.color.a;
    }

 
    private void Update() {
        transform.Translate((Vector3)textMovement * Time.deltaTime);
        textMovement -= textMovement * 8f * Time.deltaTime;

        if (timer > duration * .5f) {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        timer -= Time.deltaTime;
        if (timer < 0) 
        {
            textColor.a -= Mathf.Lerp(startAlpha, 0, Mathf.Abs(timer / timeToDisapear));
            textMesh.color = textColor;
            if (timer < -timeToDisapear) {
                Destroy(gameObject);
            }
        }
    }

}