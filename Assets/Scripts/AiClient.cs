using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DrivingTestCompletedEvent : UnityEvent<float>
{
}

public class AiClient : MonoBehaviour
{
    public DrivingTestCompletedEvent drivingTestCompleted = new DrivingTestCompletedEvent();

    public FixedSpeedCarController carController;
    public SensorData sensorData;
    public NeuralNetwork network;
    public float timeAliveMax;
    private float startTime;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (network != null)
        {
            var timeAlive = Time.time - startTime;
            if (timeAlive > timeAliveMax)
            {
                gameObject.SetActive(false);
            }

                var inputs = GetNetworkInputs();
            float[] output = network.Evaluate(inputs);

            if (Mathf.Abs(output[0]) > 0.1)
                print(output[0]);

            carController.controllerInput = new CarControllerInput
            {
                acceleration = 1f, // Mathf.Clamp(output[1], -1f, 1f),
                breaking = false,
                turning = Mathf.Clamp(output[0], -1f, 1f)
            };
        }
    }

    public void Restart(Transform startLocation, NeuralNetwork network)
    {
        carController.ResetCar(startLocation);
        this.network = network;
        gameObject.SetActive(true);
        startTime = Time.time;
    }

    public void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    private float[] GetNetworkInputs()
    {
        var carData = carController.carData;
        var carStats = carController.carStats;
        float[] inputs = new float[network.Structure[0]];
        if (network.Structure[0] == 6)
        {
            inputs = sensorData.beamDistances.Select(x => x / sensorData.beamMaxDistance).ToArray();
        }
        if (network.Structure[0] == 7)
        {
            //inputs = new float[sensorData.beamDistances.Length + 1];
            for (int i = 0; i < inputs.Length - 1; i++)
            {
                inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
            }
            inputs[inputs.Length - 1] = carData.speed / carStats.maxSpeed;
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
                inputs[inputs.Length - 2] = carData.rotation;
                inputs[inputs.Length - 1] = carData.speed / carStats.maxSpeed;
            }
            else
            {
                inputs[0] = carData.rotation;
                for (int i = 1; i < inputs.Length - 1; i++)
                {
                    inputs[i] = sensorData.beamDistances[i - 1] / sensorData.beamMaxDistance;
                }
                inputs[inputs.Length - 2] = carData.speed / carStats.maxSpeed;
            }
        }
        if (network.Structure[0] == 9)
        {
            //inputs = new float[sensorData.beamDistances.Length + 2];
            for (int i = 0; i < inputs.Length - 3; i++)
            {
                inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
            }
            inputs[inputs.Length - 3] = carData.rotation;
            inputs[inputs.Length - 2] = carData.rotation;
            inputs[inputs.Length - 1] = carData.speed / carStats.maxSpeed;
        }

        return inputs;
    }
}
