using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance;
    
    void Start()
    {
        if (distance == 0)
        {
            distance = Vector3.Distance(target.position, transform.position);
        } 
    }

    void LateUpdate()
    {
        Vector3 targetDir = transform.rotation * Vector3.back;
        Vector3 targetPosition = target.position + targetDir * distance;
        transform.position = targetPosition;
    }
}
