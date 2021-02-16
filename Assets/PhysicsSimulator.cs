using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSimulator : MonoBehaviour
{
    public OwnCarPhysics[] cars;
    public bool autoSimulate;
    [Range(1, 500)]
    public int simulationSpeed = 5;
    public float Fps = 100;
    public float FramesPerClick = 30;

    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (autoSimulate)
        {
            for (int i = 0; i < simulationSpeed; i++)
            {
                var deltaTime = 1.0f / Fps;
                foreach (var car in cars)
                {
                    car.PhysicsStep(deltaTime);
                }
                Physics.Simulate(deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var time = DateTime.Now;
            for (int i = 0; i < FramesPerClick; i++)
            {
                var deltaTime = 1.0f / Fps;
                foreach (var car in cars)
                {
                    car.PhysicsStep(deltaTime);
                }
                Physics.Simulate(deltaTime);
            }

            print($"{FramesPerClick} frames in {(DateTime.Now - time).TotalMilliseconds} ms");
        }
    }
}
