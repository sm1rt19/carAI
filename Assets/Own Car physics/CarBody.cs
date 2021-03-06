using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CarBody : MonoBehaviour
{
    public float Weight { get; set; }

    public new Rigidbody rigidbody;
    public List<CarAxle> axles;
    public Vector3 centerOfGravity;

    public CarBody()
    {
        axles.Add(new CarAxle(PositionFrontRear.Front));
        axles.Add(new CarAxle(PositionFrontRear.Rear));
    }
}

public enum PositionFrontRear
{
    Front,
    Rear
}

public enum PositionLeftRight
{
    Left,
    Right
}