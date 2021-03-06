using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CarAxle : MonoBehaviour
{
    public PositionFrontRear Position { get; }
    public float Weight { get; set; }

    public List<CarWheel> wheels;

    public CarAxle(PositionFrontRear position)
    {
        Position = position;

        wheels.Add(new CarWheel(position, PositionLeftRight.Left));
        wheels.Add(new CarWheel(position, PositionLeftRight.Right));
    }
}