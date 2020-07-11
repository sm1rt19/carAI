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
        carData.speed = 10;
    }

    void FixedUpdate()
    {
        var targetSteerAngle = ControllerInput.turning * carData.maxTurnAngle;
        float deltaSteerAngle = carData.turnAngle * Time.deltaTime;
        carData.turnAngle = Utilities.Step(carData.turnAngle, targetSteerAngle, deltaSteerAngle);

        transform.Rotate(new Vector3(0, carData.turnAngle, 0));
        var distance = carData.speed * Time.deltaTime;
        transform.Translate(Vector3.forward * distance, Space.Self);
        carData.distanceDriven += distance;
    }
}
