using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    public PositionFrontRear Position { get; }
    public PositionLeftRight Side { get; }
    public float Weight { get; set; }
    public float FrictionCoefficient { get; set; }
    public float WheelAngle
    {
        get
        {
            return WheelAngle;
        } 
        set
        {
            WheelAngle = Position == PositionFrontRear.Front ? WheelAngle = value : 0f;
        }
    }

    public CarWheel(PositionFrontRear position, PositionLeftRight side)
    {
        Position = position;
        Side = side;
    }

    public void Steering()
    {

    }
}