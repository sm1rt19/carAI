﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(OwnCarInputs))]
[RequireComponent(typeof(Rigidbody))]
public class OwnCarPhysics : MonoBehaviour
{
    public Vector3 initialSpeed;

    public Transform WheelFL;
    public Transform WheelFR;
    public Transform WheelRL;
    public Transform WheelRR;
    
    private Dictionary<Transform, Vector3> wheelPosDict = new Dictionary<Transform, Vector3>();
    private float wheelAngle = 0;

    private LineRenderer lineRenderer;

    public bool autoSimulate;
    public Transform center;
    private OwnCarInputs input;
    private Rigidbody rigidbody;
    private OwnCarSpecs specs;

    void Start()
    {
        input = GetComponent<OwnCarInputs>();
        rigidbody = GetComponent<Rigidbody>();
        specs = GetComponent<OwnCarSpecs>();
        rigidbody.centerOfMass = center.localPosition;
        rigidbody.AddRelativeForce(initialSpeed, ForceMode.VelocityChange);

        wheelPosDict[WheelFL] = WheelFL.position;
        wheelPosDict[WheelFR] = WheelFR.position;
        wheelPosDict[WheelRL] = WheelRL.position;
        wheelPosDict[WheelRR] = WheelRR.position;

        lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        ShowControls();
        if (Physics.autoSimulation)
        {
            PhysicsStep(Time.deltaTime);
        }
    }

    public void PhysicsStep(float deltaTime)
    {
        RotateWheel(deltaTime);
        CalculateWheelForce(deltaTime, WheelFL);
        CalculateWheelForce(deltaTime, WheelFR);
        CalculateWheelForce(deltaTime, WheelRR);
        CalculateWheelForce(deltaTime, WheelRL);
    }

    private void ShowControls()
    {
        var lineLenth = 10;
        var dir = new Vector3(input.horizontal, 0, input.vertical);
        var point = transform.TransformPoint(dir * lineLenth);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, point);
    }

    private void RotateWheel(float deltaTime)
    {
        wheelAngle = Utilities.Step(wheelAngle, specs.maxSteerAngle * input.horizontal, specs.steeringRate * deltaTime);
        WheelFL.localRotation = Quaternion.Euler(0, wheelAngle, 0);
        WheelFR.localRotation = Quaternion.Euler(0, wheelAngle, 0);
    }

    private Vector3 GetFrictionForce(Transform wheel)
    {
        return Vector3.zero;

        if (!wheelPosDict.ContainsKey(wheel))
            return Vector3.zero;

        var lastPos = wheelPosDict[wheel];
        var wheelVelocity = (wheel.position - lastPos);
        var carVelocityDir = wheel.InverseTransformVector(wheelVelocity);
        var forwardFactor = Vector3.Project(carVelocityDir, Vector3.forward);
        var sidewaysFactor = Vector3.Project(carVelocityDir, Vector3.right);
        var friction = specs.forwardFriction * -forwardFactor + specs.sidewaysFriction * -sidewaysFactor;
        return wheel.TransformVector(friction);
    }

    private Vector3 GetMotorForce(Transform wheel)
    {
        var force = wheel.TransformDirection(Vector3.forward) * specs.maxAcceleration * input.vertical;
        return force;
    }

    private void CalculateWheelForce(float deltaTime, Transform wheel)
    {
        var friction = GetFrictionForce(wheel);
        var motor = GetMotorForce(wheel);
        var calculatedForce = (friction + motor) * deltaTime;

        rigidbody.AddForceAtPosition(calculatedForce, wheel.position, ForceMode.Impulse);

        //var normAngle = wheelAngle / specs.maxSteerAngle;
        //var hackForce = wheel.right * normAngle * specs.maxSteerAcceleration * deltaTime;
        //var speedFactor = rigidbody.velocity.magnitude / specs.maxSpeed;
        //rigidbody.AddForceAtPosition(hackForce * speedFactor, wheel.position, ForceMode.Impulse);


        wheelPosDict[wheel] = wheel.position;

        //rigidbody.AddRelativeForce(, ForceMode.Impulse);
        //rigidbody.AddRelativeTorque(Vector3.up * specs.rotation * timeFactor * input.horizontal * specs.maxAcceleration, ForceMode.Impulse);
    }
}
