using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float acceleration;
    public float startSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float breaks;
    public float handling;
    public float rate = 0f;
    public float speed = 0f;
    public float turn = 0f;
    public float deaccelleration = 0.02f;

    public float timeAliveMax; // approximately 31 seconds to complete track in "Test scene 1", set in Unity
    public float timeAlive = 0f;
    public float distanceDriven = 0f;
    public float scoreAccelerate = 0f;
    public float scoreDecelerate = 0f;

    public DateTime startTime;
    public float secondsAlive;

    public void Speed(float rate)
    {
        this.rate = rate;
        if (rate > 0)
        {
            rate *= acceleration;
            speed = Mathf.Min(speed + rate, maxSpeed);
        }
        else
        {
            rate *= -breaks;
            Deaccelerate(rate);
        }
    }

    public void Start()
    {
        startTime = DateTime.UtcNow;
    }

    public void Accelerate()
    {
        speed = Mathf.Min(speed + acceleration, maxSpeed);
    }

    public void Reverse()
    {
        speed = Mathf.Max(speed - acceleration, -maxSpeed);
    }

    public void Break()
    {
        Deaccelerate(breaks);
    }

    private void Deaccelerate(float rate)
    {
        if (speed > 0)
        {
            speed = Mathf.Max(speed - rate, minSpeed);
        }
        else
        {
            speed = Mathf.Min(speed + rate, minSpeed);
        }
    }

    public void Turn(float rate)
    {
        turn = Mathf.Abs(speed) < 0.001 ? 0 : rate;
        transform.Rotate(new Vector3(0, turn * handling, 0));// * Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
        secondsAlive = (float)(DateTime.UtcNow - startTime).TotalSeconds;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        //Deaccelerate(deaccelleration);
    }
}
