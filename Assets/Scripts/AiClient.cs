using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiClient : MonoBehaviour
{
    public SensorData sensorData;
    private Car car;
    public NeuralNetwork network;

    void Start()
    {
        car = GetComponent<Car>();
        car.speed = car.startSpeed;
        car.turn = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //car.Accelerate();
        //var startTime = car.startTime;
        float deltaTime = Time.deltaTime;
        car.timeAlive += deltaTime;
        car.distanceDriven += car.speed * deltaTime;

        if (network != null)
        {
            float[] inputs = new float[network.Structure[0]];
            if (network.Structure[0] == 6)
            {
                inputs = sensorData.beamDistances;
                for (int i = 0; i < inputs.Length - 1; i++)
                {
                    inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
                }
            }
            if (network.Structure[0] == 7)
            {
                //inputs = new float[sensorData.beamDistances.Length + 1];
                for (int i = 0; i < inputs.Length - 1; i++)
                {
                    inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
                }
                inputs[inputs.Length - 1] = car.speed / car.maxSpeed;
            }
            if (network.Structure[0] == 8)
            {
                //inputs = new float[sensorData.beamDistances.Length + 2];
                if (true)
                {
                    for (int i = 0; i < inputs.Length - 2; i++)
                    {
                        inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
                    }
                    inputs[inputs.Length - 2] = car.turn;
                    inputs[inputs.Length - 1] = car.speed / car.maxSpeed;
                }
                else
                {
                    inputs[0] = car.turn;
                    for (int i = 1; i < inputs.Length - 1; i++)
                    {
                        inputs[i] = sensorData.beamDistances[i - 1] / sensorData.beamMaxDistance;
                    }
                    inputs[inputs.Length - 2] = car.speed / car.maxSpeed;
                }
            }
            if (network.Structure[0] == 9)
            {
                //inputs = new float[sensorData.beamDistances.Length + 2];
                for (int i = 0; i < inputs.Length - 3; i++)
                {
                    inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
                }
                inputs[inputs.Length - 3] = car.turn;
                inputs[inputs.Length - 2] = car.rate;
                inputs[inputs.Length - 1] = car.speed / car.maxSpeed;
            }
            float[] output = network.Evaluate(inputs);
            float alpha = 1f;
            float turnControl = Mathf.Clamp(alpha * output[0], -1f, 1f);
            float speedControl = car.startSpeed;
            if (network.Structure[network.Structure.Length - 1] == 2)
            {
                speedControl = Mathf.Clamp(alpha * output[1], -1f, 1f);
            }
            car.Turn(turnControl);
            car.Speed(speedControl);

            if (true)
            {
                if (speedControl > 0)
                {
                    car.scoreAccelerate += (car.maxSpeed - car.speed) / car.maxSpeed;
                    car.scoreAccelerate *= Mathf.Abs(speedControl);
                    //car.scoreAccelerate *= Mathf.Abs(turnControl);
                    //car.scoreAccelerate *= 1f - Mathf.Abs(speedControl);
                    //car.scoreAccelerate *= 1f - Mathf.Abs(turnControl);
                }
                if (speedControl < 0)
                {
                    //car.scoreDecelerate += car.speed / car.maxSpeed;
                    car.scoreDecelerate += (car.maxSpeed - car.speed) / car.maxSpeed;
                    car.scoreDecelerate *= Mathf.Abs(speedControl);
                    //car.scoreAccelerate *= Mathf.Abs(turnControl);
                }
            }
            if (false)
            {
                float upperLimitSpeed = car.maxSpeed - 20f;
                float lowerLimitSpeed = car.minSpeed + 20f;
                if (car.speed > upperLimitSpeed)
                {
                    if (speedControl > 0)
                    {
                        car.scoreAccelerate -= (car.speed - upperLimitSpeed) / (car.maxSpeed - upperLimitSpeed);
                    }
                    if (speedControl < 0)
                    {
                        car.scoreAccelerate += (car.speed - upperLimitSpeed) / (car.maxSpeed - upperLimitSpeed) * 2f;
                    }
                }
                if (car.speed < lowerLimitSpeed)
                {
                    if (speedControl > 0)
                    {
                        car.scoreAccelerate += (lowerLimitSpeed - car.speed) / lowerLimitSpeed * 2f;
                    }
                    if (speedControl < 0)
                    {
                        car.scoreAccelerate -= (lowerLimitSpeed - car.speed) / lowerLimitSpeed;
                    }
                }
            }
            if (false)
            {
                if (car.speed < car.minSpeed + 10f)
                {
                    if (speedControl > 0)
                    {
                        car.scoreAccelerate += (car.maxSpeed - car.speed) / car.maxSpeed;
                    }
                    if (speedControl < 0)
                    {
                        car.scoreAccelerate -= (car.maxSpeed - car.speed) / car.maxSpeed;
                    }
                }
                if (car.speed > car.maxSpeed - 10f)
                {
                    if (speedControl > 0)
                    {
                        car.scoreAccelerate -= car.speed / car.maxSpeed;
                    }
                    if (speedControl < 0)
                    {
                        car.scoreAccelerate += car.speed / car.maxSpeed;
                    }
                }
            }
        }


        //print("===================================================================================");
        //print("beam hits: " + string.Join(", ", inputs));
        //print("botcont  : " + string.Join(", ", botCont));
        //print("usercont : " + string.Join(", ", userCont));
    }

    public Car GetCar()
    {
        return car;
    }
}
