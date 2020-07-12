using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CarControllerBase : MonoBehaviour
{
    public CarControllerInput controllerInput;

    public CarStats carStats;

    public CarData carData;

    public virtual void ResetCar(Transform location)
    {
        transform.position = location.position;
        transform.rotation = location.rotation;
        carData = new CarData
        {
            distanceDriven = 0,
            rotation = 0,
            speed = 0
        };
    }
}

[Serializable]
public struct CarControllerInput
{
    public float horizontal;
    public float vertical;
    public bool breaking;
}

[Serializable]
public struct CarStats
{
    public float acceleration;
    public float maxSpeed;
    public float maxRotation;
    public float rateRotation;
}

[Serializable]
public class CarData
{
    public float speed;
    public float rotation;
    public float distanceDriven;
}