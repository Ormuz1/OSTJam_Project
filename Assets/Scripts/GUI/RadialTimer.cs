using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class RadialTimer : MonoBehaviour
{
    private Image radial;
    public float duration;
    private float timer = 0;
    [SerializeField] private UnityEvent OnFinished;
    private void Awake() 
    {
        radial = GetComponent<Image>();
    }

    private void Update() 
    {
        if(timer < duration)
        {
            radial.fillAmount = 1 - timer / duration;
            timer += Time.deltaTime;
        }  
        else
        {
            OnFinished.Invoke();
        }
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
