﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorData : MonoBehaviour
{
    [Header("Setup")]
    public float beamMaxDistance;
    private Beam[] beams;

    [Header("Data")]
    public float speed;
    public float angleToCheckpoint;
    public float[] beamDistances;

    // Start is called before the first frame update
    void Start()
    {
        beams = GetComponentsInChildren<Beam>();
        beamDistances = new float[beams.Length];
        foreach (var beam in beams)
        {
            beam.SetBeamMaxDistance(beamMaxDistance);
        }
        PhysicsSimulator.instance.onPrePhysicsStep.AddListener(MeasureBeams);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        PhysicsSimulator.instance.onPrePhysicsStep.RemoveListener(MeasureBeams);
    }

    private void MeasureBeams(float deltaTime)
    {
        for (var i = 0; i < beams.Length; i++)
        {
            if (beams[i].gameObject.activeSelf)
                beamDistances[i] = beams[i].Measure();
        }
    }
}
