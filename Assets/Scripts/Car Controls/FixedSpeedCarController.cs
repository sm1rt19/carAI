using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSpeedCarController : CarControllerBase 
{
    void Start()
    {
        PhysicsSimulator.instance.onPhysicsStep.AddListener(PhysicsStep);
    }

    void OnDestroy()
    {
        PhysicsSimulator.instance.onPhysicsStep.RemoveListener(PhysicsStep);
    }

    void PhysicsStep(float deltaTime)
    {
        float targetRotation = controllerInput.horizontal * carStats.maxRotation;
        float deltaRotation = carStats.rateRotation * deltaTime;
        carData.rotation = Utilities.Step(carData.rotation, targetRotation, deltaRotation);

        transform.Rotate(new Vector3(0, carData.rotation, 0));
        var distance = carStats.maxSpeed * deltaTime;
        transform.Translate(Vector3.forward * distance, Space.Self);
        carData.distanceDriven += distance;
    }

    public override void ResetCar(Transform location)
    {
        base.ResetCar(location);
        carData.speed = carStats.maxSpeed;
    }
}
