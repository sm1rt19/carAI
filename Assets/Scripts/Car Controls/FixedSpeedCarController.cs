using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSpeedCarController : CarControllerBase 
{
    void FixedUpdate()
    {
        float targetRotation = controllerInput.turning * carStats.maxRotation;
        float deltaRotation = carStats.rateRotation * Time.deltaTime;
        carData.rotation = Utilities.Step(carData.rotation, targetRotation, deltaRotation);

        transform.Rotate(new Vector3(0, carData.rotation, 0));
        var distance = carData.speed * Time.deltaTime;
        transform.Translate(Vector3.forward * distance, Space.Self);
        carData.distanceDriven += distance;
    }

    public override void ResetCar(Transform location)
    {
        base.ResetCar(location);
        carData.speed = carStats.maxSpeed;
    }
}
