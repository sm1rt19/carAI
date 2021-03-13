using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public Vector3 positionOffset;
    public Vector3 lookOffset;
    public float stiffness = 0.1f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target == null)
            Debug.LogError("Camera could not find target");
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            var targetPoint = target.TransformPoint(positionOffset);
            transform.Translate((targetPoint - transform.position) * stiffness, Space.World);

            targetPoint = target.TransformPoint(lookOffset);
            transform.LookAt(targetPoint);
        }
    }
}
