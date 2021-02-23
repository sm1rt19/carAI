﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all static specification of the car. Meaning all data of the car when NOT driving. 
/// </summary>
public class OwnCarSpecs : MonoBehaviour
{
    [Header("Power")]
    public AnimationCurve accelerationCurve;
    public float maxSpeed;
    public float acceleration;

    [Header("Steering")]
    public float maxSteerAngle;
    public float steeringRate;
    public float maxSteerAcceleration;

    [Header("Friction")]
    public float forwardFriction;
    public float sidewaysFriction;
    public float rotationalFriction;
}