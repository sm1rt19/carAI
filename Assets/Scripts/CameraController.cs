using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 positionOffset;
    public Vector3 lookOffset;
    public float stiffness = 0.1f;

    // Update is called once per frame
    void Update()
    {
        var targetPoint = target.TransformPoint(positionOffset);
        transform.Translate((targetPoint - transform.position) * stiffness, Space.World);
        
        targetPoint = target.TransformPoint(lookOffset);
        transform.LookAt(targetPoint);
    }
}
