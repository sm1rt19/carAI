using System.Collections;
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

    [Header("Break")]
    public float breakingAcceleration;

    [Header("Steering")]
    public float maxSteerAngle;
    public float steeringRate;
    public float maxSteerAcceleration;
    public AnimationCurve steeringCurve;

    [Header("Friction")]
    public float forwardFriction;
    public float sidewaysFriction;
    public float rotationalFriction;

    [Header("Carsten's Specs")]
    public Vector3 gravity;
    [Range(0f, 1f)]
    public float limitBySpeed = 0.5f;
    public float maxMotorForce;
    public float airResistanceCoefficient;
    public float rollingResistanceCoefficient;
    public float staticFrictionCoefficient;
    public float dynamicFrictionCoefficient;
}
