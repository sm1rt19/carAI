using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableSpeedCarController : CarControllerBase 
{
    void FixedUpdate()
    {
        // update speed
        if (controllerInput.breaking)
        {
            float targetSpeed = 0;
            float deltaSpeed = 3 * carStats.acceleration * Time.deltaTime;
            carData.speed = Utilities.Step(carData.speed, targetSpeed, deltaSpeed);
        }
        else
        {
            float targetSpeed = controllerInput.vertical * carStats.maxSpeed;
            float deltaSpeed = carStats.acceleration * Time.deltaTime;
            carData.speed = Utilities.Step(carData.speed, targetSpeed, deltaSpeed);
        }


        // update rotation
        float targetRotation = controllerInput.horizontal * carStats.maxRotation;
        float deltaRotation = carStats.rateRotation * Time.deltaTime;
        carData.rotation = Utilities.Step(carData.rotation, targetRotation, deltaRotation);



        // move car
        var rotationSpeedFactor = Mathf.Clamp(Mathf.Abs(carData.speed) / 15f, 0, 1);
        transform.Rotate(new Vector3(0, carData.rotation * rotationSpeedFactor, 0));
        var distance = carData.speed * Time.deltaTime;
        transform.Translate(Vector3.forward * distance, Space.Self);
        carData.distanceDriven += distance;
    }
}
