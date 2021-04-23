using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float timeToReachTarget;
    [SerializeField] private float zOffset;
    private Vector3 velocity = Vector3.zero;
    private Transform target;
    [HideInInspector] public bool isFollowing = false;


    private void Start()
    {
        target = UnitManager.Instance.allies[0].transform;
    }


    private void Update() 
    {
        if(isFollowing)
        {
            Vector3 desiredPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                target.position.z + zOffset
            );

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, timeToReachTarget);
        }
    }
}
