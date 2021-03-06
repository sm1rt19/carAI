using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(OwnCarInputs))]
[RequireComponent(typeof(Rigidbody))]
public class OwnCarPhysics : MonoBehaviour
{
    public Vector3 initialSpeed;

    public Transform WheelFL;
    public Transform WheelFR;
    public Transform WheelRL;
    public Transform WheelRR;

    private Dictionary<Transform, Vector3> wheelPosDict = new Dictionary<Transform, Vector3>();
    private float wheelAngle = 0f;

    private LineRenderer lineRenderer;
    private LineRenderer motorForce;

    public bool autoSimulate;
    public Transform center;
    private OwnCarInputs input;
    private Rigidbody rigidbody;
    private OwnCarSpecs specs;

    private DateTime startTime;

    void Start()
    {
        input = GetComponent<OwnCarInputs>();
        rigidbody = GetComponent<Rigidbody>();
        specs = GetComponent<OwnCarSpecs>();
        rigidbody.centerOfMass = center.localPosition;
        rigidbody.AddRelativeForce(initialSpeed, ForceMode.VelocityChange);

        wheelPosDict[WheelFL] = WheelFL.position;
        wheelPosDict[WheelFR] = WheelFR.position;
        wheelPosDict[WheelRL] = WheelRL.position;
        wheelPosDict[WheelRR] = WheelRR.position;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        ShowControls();
        if (Physics.autoSimulation)
        {
            PhysicsStep(Time.deltaTime);
        }
    }

    public void PhysicsStep(float deltaTime)
    {
        RotateWheel(deltaTime);
        CalculateNormalPhysics(deltaTime);


        //CalculateWheelForce(deltaTime, WheelFL);
        //CalculateWheelForce(deltaTime, WheelFR);

        //CalculateWheelForce(deltaTime, WheelRR);
        //CalculateWheelForce(deltaTime, WheelRL);
    }

    public void CalculateDriftPhysics(float deltaTime)
    {
        BasicRotationalFriction(deltaTime);
    }

    public void CalculateNormalPhysics(float deltaTime)
    {
        if (input.breaking)
        {
            BasicBreak(deltaTime);
        }
        else
        {
            BasicAccelerate(deltaTime);
        }
        //BasicFriction(deltaTime);
        CalculateRotation(deltaTime);
    }

    private void BasicBreak(float deltaTime)
    {
        var frictionForce = Mathf.Min(rigidbody.velocity.magnitude, specs.breakingAcceleration) * -rigidbody.velocity.normalized * deltaTime;
        rigidbody.AddForce(frictionForce, ForceMode.VelocityChange);
    }

    private void CalculateRotation(float deltaTime)
    {
        if (false)
        {
            var relativeSpeed = transform.InverseTransformVector(rigidbody.velocity).z;
            var rotationPerSpeed = specs.maxSteerAcceleration * specs.steeringCurve.Evaluate(Mathf.Abs(relativeSpeed) / specs.maxSpeed);
            var rotationForce = wheelAngle * deltaTime * relativeSpeed * rotationPerSpeed;
            rigidbody.angularVelocity = new Vector3(0, rotationForce, 0);
            //rigidbody.AddTorque(0, rotationForce, 0, ForceMode.VelocityChange);
        }
        else
        {
            float omega = GetAngularVelocity(deltaTime);
            float R = transform.InverseTransformVector(rigidbody.velocity).z / omega;
            rigidbody.angularVelocity = new Vector3(0, omega, 0);
        }
    }

    private void BasicAccelerate(float deltaTime)
    {
        bool UseCarstenForce = false;
        if (UseCarstenForce)
        {
            Vector3 acceleration = GetTotalForceVector(WheelRR) / rigidbody.mass;
            rigidbody.velocity += acceleration * deltaTime;
        }
        else
        {
            float acceleration = specs.acceleration * specs.accelerationCurve.Evaluate(rigidbody.velocity.magnitude / specs.maxSpeed);
            if (acceleration > 0)
            {
                float velocityChange = input.vertical * acceleration * deltaTime;
                float velocity = transform.InverseTransformVector(rigidbody.velocity).z + velocityChange;
                //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                rigidbody.velocity = transform.forward * velocity;
            }
        }
    }

    private void BasicFriction(float deltaTime)
    {
        if (rigidbody.velocity == Vector3.zero)
            return;

        var forwardFactor = Vector3.Project(rigidbody.velocity.normalized, transform.forward).magnitude;
        var sidewaysFactor = Vector3.Project(rigidbody.velocity.normalized, transform.right).magnitude;
        var frictionDir = -rigidbody.velocity.normalized;
        var friction = specs.sidewaysFriction * sidewaysFactor + specs.forwardFriction * forwardFactor;
        var frictionForce = Mathf.Min(rigidbody.velocity.magnitude, friction) * frictionDir * deltaTime;
        rigidbody.AddForce(frictionForce, ForceMode.VelocityChange);
    }

    private void BasicRotationalFriction(float deltaTime)
    {
        if (rigidbody.angularVelocity.magnitude == 0)
            return;

        var friction = Mathf.Min(rigidbody.angularVelocity.magnitude, specs.rotationalFriction);
        var frictionForce = friction * -rigidbody.angularVelocity.normalized * deltaTime;
        rigidbody.AddTorque(frictionForce, ForceMode.VelocityChange);
    }

    private void ShowControls()
    {
        var lineLenth = input.vertical * 10f;
        //var dir = new Vector3(input.horizontal, 0, input.vertical);
        var dir = new Vector3(Mathf.Sign(input.vertical) * Mathf.Cos(Mathf.PI / 2f - wheelAngle / 180f * Mathf.PI), 0, Mathf.Sin(Mathf.PI / 2f - wheelAngle / 180f * Mathf.PI));
        var point = transform.TransformPoint(dir * lineLenth);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, point);
        lineRenderer.material.color = !input.breaking ? Color.blue : Color.red;
    }

    private void RotateWheel(float deltaTime)
    {
        wheelAngle = Utilities.Step(wheelAngle, LimitbyVelocity(specs.maxSteerAngle, specs.limitBySpeed, deltaTime) * input.horizontal, specs.steeringRate * deltaTime);
        //wheelAngle = Utilities.Step(wheelAngle, specs.maxSteerAngle * input.horizontal, LimitbyVelocity(specs.steeringRate, specs.limitBySpeed, deltaTime) * deltaTime);
        WheelFL.localRotation = Quaternion.Euler(0, wheelAngle, 0);
        WheelFR.localRotation = Quaternion.Euler(0, wheelAngle, 0);
    }

    private Vector3 GetFrictionForce(Transform wheel)
    {
        return Vector3.zero;

        if (!wheelPosDict.ContainsKey(wheel))
            return Vector3.zero;

        var lastPos = wheelPosDict[wheel];
        var wheelVelocity = (wheel.position - lastPos);
        var carVelocityDir = wheel.InverseTransformVector(wheelVelocity);
        var forwardFactor = Vector3.Project(carVelocityDir, Vector3.forward);
        var sidewaysFactor = Vector3.Project(carVelocityDir, Vector3.right);
        var friction = specs.forwardFriction * -forwardFactor + specs.sidewaysFriction * -sidewaysFactor;
        return wheel.TransformVector(friction);
    }

    //private Vector3 GetFrictionForceCar()
    //{
    //    rigidbody.velocity.
    //}

    private Vector3 GetMotorForce(float deltaTime, Transform wheel)
    {
        var force = wheel.TransformDirection(Vector3.forward) * specs.acceleration * input.vertical * deltaTime;
        rigidbody.AddForceAtPosition(force, wheel.position, ForceMode.VelocityChange);
        return force;
    }

    private void CalculateWheelForce(float deltaTime, Transform wheel)
    {
        //var friction = GetFrictionForce(wheel);
        var motor = GetMotorForce(deltaTime, wheel);
        var calculatedForce = (motor) * deltaTime;



        //var normAngle = wheelAngle / specs.maxSteerAngle;
        //var hackForce = wheel.right * normAngle * specs.maxSteerAcceleration * deltaTime;
        //var speedFactor = rigidbody.velocity.magnitude / specs.maxSpeed;
        //rigidbody.AddForceAtPosition(hackForce * speedFactor, wheel.position, ForceMode.Impulse);


        wheelPosDict[wheel] = wheel.position;

        //rigidbody.AddRelativeForce(, ForceMode.Impulse);
        //rigidbody.AddRelativeTorque(Vector3.up * specs.rotation * timeFactor * input.horizontal * specs.maxAcceleration, ForceMode.Impulse);
    }

    private float GetAngularVelocity(float deltaTime)
    {
        float L = 1.541f;// Math.Abs(WheelFL.transform.position.z - WheelRL.transform.position.z);
        return (Mathf.Deg2Rad * wheelAngle - Mathf.Asin((L - transform.InverseTransformVector(rigidbody.velocity).z * deltaTime) / L * Mathf.Sin(Mathf.PI - Mathf.Deg2Rad * wheelAngle))) / deltaTime;
    }

    private float LimitbyVelocity(float parameter, float reduction, float deltaTime)
    {
        float omega = GetAngularVelocity(deltaTime);
        if (Math.Abs(omega) > 0f)
        {
            //parameter *= 1f - Math.Abs(transform.InverseTransformVector(rigidbody.velocity).z * omega);
            parameter *= 1f - Math.Abs(transform.InverseTransformVector(rigidbody.velocity).z / specs.maxSpeed) * reduction;
        }
        return parameter;
    }

    private Vector3 GetLocalVelocityVector(string direction)
    {
        Vector3 velocity = new Vector3(0f, 0f, 0f);
        switch (direction)
        {
            case "x":
                velocity.x = transform.InverseTransformVector(rigidbody.velocity).x;
                break;
            case "y":
                velocity.y = transform.InverseTransformVector(rigidbody.velocity).y;
                break;
            case "z":
                velocity.z = transform.InverseTransformVector(rigidbody.velocity).z;
                break;
            case "xy":
                velocity.x = transform.InverseTransformVector(rigidbody.velocity).x;
                velocity.y = transform.InverseTransformVector(rigidbody.velocity).y;
                break;
            case "xz":
                velocity.x = transform.InverseTransformVector(rigidbody.velocity).x;
                velocity.z = transform.InverseTransformVector(rigidbody.velocity).z;
                break;
            case "yz":
                velocity.y = transform.InverseTransformVector(rigidbody.velocity).y;
                velocity.z = transform.InverseTransformVector(rigidbody.velocity).z;
                break;
            case "xyz":
                velocity.x = transform.InverseTransformVector(rigidbody.velocity).x;
                velocity.y = transform.InverseTransformVector(rigidbody.velocity).y;
                velocity.z = transform.InverseTransformVector(rigidbody.velocity).z;
                break;
            default:
                throw new NotImplementedException("Invalid direction \"" + direction + "\".");
        }
        return velocity;
    }

    private float GetLocalVelocityMagnitude(string direction)
    {
        return GetLocalVelocityVector(direction).magnitude;
    }

    private Vector3 GetTotalForceVector(Transform wheel)
    {
        Vector3 force = GetMotorForceVector(wheel);
        force += GetAirResistanceVector();
        //force += GetRollingResistanceVector();
        //DrawVector(GetMotorForceVector(wheel),Color.green, 0.001f);
        DrawVector(GetAirResistanceVector(), Color.green, 0.1f);
        //DrawVector(GetRollingResistanceVector(), Color.green, 0.1f);
        //DrawVector(force, Color.green, 0.01f);
        return force;
    }

    private Vector3 GetMotorForceVector(Transform wheel)
    {
        return wheel.TransformDirection(Vector3.forward) * specs.maxMotorForce * input.vertical;
    }

    private Vector3 GetAirResistanceVector()
    {
        Vector3 direction = -transform.InverseTransformDirection(rigidbody.velocity).normalized;
        float magnitude = transform.InverseTransformDirection(rigidbody.velocity).magnitude;
        return specs.airResistanceCoefficient * magnitude * magnitude * direction;
        // Normalized equals direction / magnitude, use for following short expression:
        //return -specs.airResistanceCoefficient * transform.InverseTransformDirection(rigidbody.velocity).magnitude * transform.InverseTransformDirection(rigidbody.velocity);
    }

    private Vector3 GetRollingResistanceVector() 
    {
        Vector3 direction = -transform.InverseTransformDirection(rigidbody.velocity).normalized;
        direction.x = 0f;
        direction.y = 0f;
        direction.z = Mathf.Abs(direction.z) < 0.000001f ? 0f : direction.z;
        return specs.rollingResistanceCoefficient * rigidbody.mass * Mathf.Abs(specs.gravity.y) * direction;
    }

    private void DrawVector(Vector3 vector, Color color, float multiplier = 1f)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.TransformPoint(vector * multiplier));
        lineRenderer.material.color = color;
    }
}