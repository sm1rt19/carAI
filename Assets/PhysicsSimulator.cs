using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PhysicsStepEvent : UnityEvent<float> { }

public class PhysicsSimulator : MonoBehaviour
{
    public static PhysicsSimulator instance;
    [HideInInspector]
    public PhysicsStepEvent onPrePhysicsStep = new PhysicsStepEvent();
    [HideInInspector]
    public PhysicsStepEvent onPhysicsStep = new PhysicsStepEvent();

    public bool debug = true;

    [Header("Simulation")]
    public int actualSpeed;
    public bool paused;
    [Tooltip("How often does the physics update (updates per second)")]
    public float updateRate = 50;
    [Tooltip("The number of simulation steps per second with a simulation speed of 1")]
    public float defaultFps = 100;
    [Tooltip("The simulation speed. 2 = double speed, 0.5 = half speed")]
    public float simulationSpeed = 1f;

    private float restFrames;
    private Queue<int> simulatedStepsQueue = new Queue<int>();
    private Stopwatch watch = new Stopwatch();

    void Awake()
    {
        instance = this;
        Physics.autoSimulation = false;
        StartCoroutine(Simulate());
    }

    public void UnpauseSimulation()
    {
        paused = false;
    }

    public void PauseSimulation()
    {
        paused = true;
    }

    public void SetSimulationSpeed(float simulationSpeed)
    {
        this.simulationSpeed = simulationSpeed;
    }

    private IEnumerator Simulate()
    {
        while (true)
        {
            if (paused)
            {
                yield return null;
                continue;
            }

            watch.Restart();

            var timePerUpdateMs = 1000 / updateRate;

            // in case we use full speed simulation speed we have to sleep a bit to not overload the cpu
            var maxTimeMs = timePerUpdateMs - 1;

            // calculate the target number of steps to simulate in this update
            var deltaTime = 1 / defaultFps;
            var targetFrames = simulationSpeed * timePerUpdateMs / (deltaTime * 1000) + restFrames;
            restFrames = targetFrames % 1;

            // run the simulation
            var simulatedSteps = 0;

            while (watch.ElapsedMilliseconds < maxTimeMs && simulatedSteps < (int)targetFrames)
            {
                onPrePhysicsStep.Invoke(deltaTime);
                onPhysicsStep.Invoke(deltaTime);
                Physics.Simulate(deltaTime);
                simulatedSteps++;
            }

            // update the field displaying the actual fpsa
            if (debug)
            {
                simulatedStepsQueue.Enqueue(simulatedSteps);
                if (simulatedStepsQueue.Count > 10)
                    simulatedStepsQueue.Dequeue();
                actualSpeed = (int)(simulatedStepsQueue.Average() * updateRate / defaultFps);
            }

            var waitTimeMs = 1000 / updateRate - watch.ElapsedMilliseconds;
            yield return new WaitForSecondsRealtime(waitTimeMs / 1000f);
        }
    }
}
