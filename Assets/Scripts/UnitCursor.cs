using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCursor : MonoBehaviour
{
    private Unit followUnit;
    [SerializeField] private Vector3 offset;
    private void Update() 
    {
        transform.position = followUnit.transform.position + offset;
    }

    public void FollowNewUnit(Unit unit)
    {
        followUnit = unit;
    }
}
