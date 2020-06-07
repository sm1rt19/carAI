using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire2Wheel : MonoBehaviour
{
    private WheelCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = this.GetComponentInParent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        collider.GetWorldPose(out Vector3 newPos, out Quaternion newRotation);
        transform.position = newPos;
        transform.rotation = newRotation;
    }
}
