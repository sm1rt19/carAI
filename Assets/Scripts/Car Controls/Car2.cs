using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WheelSetup
{
    public WheelCollider collider;
    public Transform model;
}

public class Car2 : MonoBehaviour
{
    public AnimationCurve maxMotorTorqueCurve;

    public float maxMotorRPM = 7000f;
    public float currentMotorRPM;

    public float maxMotorTorque = 400f;
    public float motorTorqueRate = 100f;
    public float currentMaxMotorTorque;
    public float currentMotorTorque;
    public float targetMotorTorque;

    public float maxSteeringAngle = 40f;
    public float steeringAngleRate = 50f;
    public float currentSteeringAngle;

    public float frontWheelRPMLeft;
    public float frontWheelRPMRight;
    public float rearWheelRPMLeft;
    public float rearWheelRPMRight;

    private float inputX;
    private float inputY;

    public WheelSetup fl;
    public WheelSetup fr;
    public WheelSetup bl;
    public WheelSetup br;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxisRaw("Speeder");

        Steer();
        Accelerate();

        // update wheel poses
        UpdateWheelPose(fr);
        UpdateWheelPose(fl);
        UpdateWheelPose(br);
        UpdateWheelPose(bl);

        frontWheelRPMLeft = fl.collider.rpm;
        frontWheelRPMRight = fr.collider.rpm;
        rearWheelRPMLeft = bl.collider.rpm;
        rearWheelRPMRight = br.collider.rpm;
    }


    private void Steer()
    {
        var steerAngle = maxSteeringAngle * inputX;
        fl.collider.steerAngle = steerAngle;
        fr.collider.steerAngle = steerAngle;
    }

    private void Accelerate()
    {
        currentMotorRPM = (bl.collider.rpm + br.collider.rpm) / 2f * 10f;
        currentMaxMotorTorque = maxMotorTorqueCurve.Evaluate(currentMotorRPM / maxMotorRPM) * maxMotorTorque;
        targetMotorTorque = currentMaxMotorTorque * inputY;
        //float targetMotorTorque = maxMotorTorque * inputY;
        float deltaMotorTorque = motorTorqueRate * Time.deltaTime;
        currentMotorTorque = Step(currentMotorTorque, targetMotorTorque, deltaMotorTorque);
        //fl.collider.motorTorque = currentMotorTorque;
        //fr.collider.motorTorque = currentMotorTorque;
        bl.collider.motorTorque = currentMotorTorque;
        br.collider.motorTorque = currentMotorTorque;
    }

    private static float Step(float currentValue, float targetValue, float delta)
    {
        if (targetValue > currentValue)
        {
            delta = Mathf.Min(delta, targetValue - currentValue);
            currentValue += delta;
        }
        if (targetValue < currentValue)
        {
            delta = Mathf.Min(delta, currentValue - targetValue);
            currentValue -= delta;
        }
        return currentValue;
    }

    private void UpdateWheelPose(WheelSetup wheelSetup)
    {
        wheelSetup.collider.GetWorldPose(out Vector3 pos, out Quaternion rotation);
        wheelSetup.model.position = pos;
        wheelSetup.model.rotation = rotation;
    }
}