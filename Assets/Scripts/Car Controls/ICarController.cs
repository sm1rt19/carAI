using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ICarController
{
    CarControllerInput ControllerInput { get; set; }
    CarData CarData { get; }
}

[Serializable]
public struct CarControllerInput
{
    public float turning;
    public float acceleration;
    public bool breaking;
}

[Serializable]
public struct CarData
{
    public float speed;
    public float maxSpeed;
    public float rotation;
    public float maxRotation;
    public float rateRotation;

    public float distanceDriven;
}