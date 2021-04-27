using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCursor : MonoBehaviour
{
    [SerializeField] private Unit followUnit;
    [SerializeField] private Vector3 offset;

    private void Update() 
    {
        if(followUnit)
            transform.position = followUnit.transform.position + followUnit.unitCursorOffset + Vector3.up * followUnit.meshBounds.size.y;
        else
            gameObject.SetActive(false);
    }

    public void FollowNewUnit(Unit unit)
    {
        followUnit = unit;
        transform.position = followUnit.transform.position + followUnit.unitCursorOffset + Vector3.up * followUnit.meshBounds.size.y;
    }
}
