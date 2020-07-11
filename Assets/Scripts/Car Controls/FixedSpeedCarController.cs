using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSpeedCarController : MonoBehaviour, ICarController 
{
    public CarControllerInput ControllerInput { get; set; }

    private CarData carData;
    public CarData CarData => carData;

    public void Start()
    {
        carData.speed = 20;
        carData.maxSpeed = 50;
        carData.rotation = 0;
        carData.maxRotation = 20;
        carData.rateRotation = 200;
    }

    void FixedUpdate()
    {
        float targetRotation = ControllerInput.turning * carData.maxRotation;
        float deltaRotation = carData.rateRotation * Time.deltaTime;
        carData.rotation = Utilities.Step(carData.rotation, targetRotation, deltaRotation);

        transform.Rotate(new Vector3(0, carData.rotation, 0));
        var distance = carData.speed * Time.deltaTime;
        transform.Translate(Vector3.forward * distance, Space.Self);
        carData.distanceDriven += distance;
    }
}
