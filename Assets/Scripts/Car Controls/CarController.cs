using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DriveMode
{
    Front = 1,
    Rear = 2,
    All = Front | Rear
}

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public CarControllerInput input;

    public AnimationCurve torqueCurve;
    public float maxRpm = 1000f;
    public float maxTorque = 1000f;
    public Transform centerOfGravity;

    public WheelCollider wheelFR;
    public WheelCollider wheelFL;
    public WheelCollider wheelRR;
    public WheelCollider wheelRL;

    public float maxSteerAngle = 40f;
    public float rateSteerAngle = 100f;
    public float currentSteerAngle;
    public float targetSteerAngle;
    public float brakeTorque = 1000f;

    public float AntiRoll = 20000.0f;
    public DriveMode driveMode = DriveMode.Rear;

    private Rigidbody rigidbody;

    public float WheelRpm
    {
        get
        {
            List<float> rpms = new List<float>();
            if (driveMode.HasFlag(DriveMode.Front))
            {
                rpms.Add(wheelFL.rpm);
                rpms.Add(wheelFR.rpm);
            }
            if (driveMode.HasFlag(DriveMode.Rear))
            {
                rpms.Add(wheelRL.rpm);
                rpms.Add(wheelRR.rpm);
            }

            return rpms.Average();
        }
    }

    public float WheelSpeed => wheelFR.radius * Mathf.PI * WheelRpm * 60f / 1000f;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centerOfGravity.localPosition;
    }

    void FixedUpdate()
    {
        DoRollBar(wheelFR, wheelFL);
        DoRollBar(wheelRR, wheelRL);

        //wheelFR.steerAngle = input.horizontal * maxSteerAngle;
        //wheelFL.steerAngle = wheelFR.steerAngle;

        targetSteerAngle = input.horizontal * maxSteerAngle;
        float deltaSteerAngle = rateSteerAngle * Time.deltaTime;
        currentSteerAngle = Utilities.Step(currentSteerAngle, targetSteerAngle, deltaSteerAngle);
        wheelFR.steerAngle = currentSteerAngle;
        wheelFL.steerAngle = wheelFR.steerAngle;

        var torque = input.vertical * torqueCurve.Evaluate(Mathf.Abs(WheelRpm / maxRpm)) * maxTorque;
        wheelFL.motorTorque = driveMode.HasFlag(DriveMode.Front) ? torque : 0;
        wheelFR.motorTorque = driveMode.HasFlag(DriveMode.Front) ? torque : 0;
        wheelRR.motorTorque = driveMode.HasFlag(DriveMode.Rear) ? torque : 0;
        wheelRL.motorTorque = driveMode.HasFlag(DriveMode.Rear) ? torque : 0;

        if (input.breaking)
        {
            wheelFR.brakeTorque = brakeTorque;
            wheelFL.brakeTorque = brakeTorque;
            wheelRR.brakeTorque = brakeTorque;
            wheelRL.brakeTorque = brakeTorque;
        }
        else
        {
            wheelFR.brakeTorque = 0;
            wheelFL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
            wheelRL.brakeTorque = 0;
        }
    }

    private void DoRollBar(WheelCollider WheelL, WheelCollider WheelR)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

        bool groundedR = WheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            rigidbody.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                WheelL.transform.position);
        if (groundedR)
            rigidbody.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                WheelR.transform.position);
    }

    
}
