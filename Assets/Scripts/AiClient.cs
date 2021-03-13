using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DrivingTestCompletedEvent : UnityEvent<int, bool, float, float> { }

public class AiClient : MonoBehaviour
{
    [HideInInspector]
    public DrivingTestCompletedEvent drivingTestCompleted = new DrivingTestCompletedEvent();
    [HideInInspector]
    public int id;
    public CarControllerBase carController;
    public SensorData sensorData;
    public NeuralNetwork network;
    public float timeAliveMax;
    private float startTime;
    private bool checkSpeed = false;

    void Start()
    {
        carController = GetComponent<CarControllerBase>();
        PhysicsSimulator.instance.onPrePhysicsStep.AddListener(UpdateAiControls);
    }

    private void EnableSpeedCheck()
    {
        checkSpeed = true;
    }

    void OnDestroy()
    {
        PhysicsSimulator.instance.onPrePhysicsStep.RemoveListener(UpdateAiControls);
    }

    // Update is called once per frame
    void UpdateAiControls(float deltaTime)
    {
        if (network != null)
        {
            var timeAlive = Time.time - startTime;
            if (checkSpeed && carController.carData.speed < 5)
            {
                drivingTestCompleted.Invoke(id, false, timeAlive, carController.carData.distanceDriven);
            }

            if (carController.carData.speed < 0)
            {
                drivingTestCompleted.Invoke(id, false, timeAlive, carController.carData.distanceDriven);
            }

            if (timeAlive > timeAliveMax)
            {
                drivingTestCompleted.Invoke(id, false, timeAlive, carController.carData.distanceDriven);
            }

            float[] inputs = GetNetworkInputs();
            float[] output = network.Evaluate(inputs);
            carController.controllerInput = new CarControllerInput
            {
                vertical = output.Length > 1 ? output[1] : -1000, // Mathf.Clamp(output[1], -1f, 1f),
                breaking = false,
                horizontal = output[0]
            };
        }
    }

    public void Restart(Transform startLocation, NeuralNetwork network)
    {
        carController.ResetCar(startLocation);
        this.network = network;
        gameObject.SetActive(true);
        startTime = Time.time;
        checkSpeed = false;
        Invoke("EnableSpeedCheck", 3);
    }

    public void OnCollisionEnter(Collision collision)
    {
        var timeAlive = Time.time - startTime;
        var completed = false;
        //completed = timeAlive > 10f;
        //completed = collision.collider.CompareTag("Goal line") && timeAlive > 10f;
        completed = collision.collider.CompareTag("Goal line");
        drivingTestCompleted.Invoke(id, completed, timeAlive, carController.carData.distanceDriven);
    }

    private float[] GetNetworkInputs()
    {
        var carData = carController.carData;
        var carStats = carController.carStats;
        float[] inputs = new float[network.Structure[0]];

        if (!sensorData.beamDistances.Any())
            return inputs;

        if (network.Structure[0] == 6)
        {
            inputs = sensorData.beamDistances.Select(x => x / sensorData.beamMaxDistance).ToArray();
        }
        if (network.Structure[0] == 7)
        {
            //inputs = new float[sensorData.beamDistances.Length + 1];
            for (int i = 0; i < inputs.Length - 1; i++)
            {
                if (i >= sensorData.beamDistances.Length)
                    inputs[i] = 0; 
                else
                    inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
            }
            inputs[inputs.Length - 1] = carData.speed / carStats.maxSpeed;
        }
        if (network.Structure[0] == 8 || network.Structure[0] == 14)
        {
            for (int i = 0; i < inputs.Length - 2; i++)
            {
                if (i >= sensorData.beamDistances.Length)
                    inputs[i] = 0;
                else
                    inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
            }
            inputs[inputs.Length - 2] = carData.rotation / carStats.maxRotation;
            inputs[inputs.Length - 1] = carData.speed / carStats.maxSpeed;

        }
        if (network.Structure[0] == 9)
        {
            //inputs = new float[sensorData.beamDistances.Length + 2];
            for (int i = 0; i < inputs.Length - 3; i++)
            {
                if (i >= sensorData.beamDistances.Length)
                    inputs[i] = 0;
                else
                    inputs[i] = sensorData.beamDistances[i] / sensorData.beamMaxDistance;
            }
            inputs[inputs.Length - 3] = carData.rotation;
            inputs[inputs.Length - 2] = carData.rotation;
            inputs[inputs.Length - 1] = carData.speed / carStats.maxSpeed;
        }

        return inputs;
    }
}
