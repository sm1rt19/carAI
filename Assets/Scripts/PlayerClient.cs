using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PlayerClient : MonoBehaviour
{
    private CarController controller;
    public bool ps4;

    void Start()
    {
        controller = GetComponent<CarController>();
    }

    void Update()
    {
        if (ps4)
        {
            controller.input = new CarControllerInput
            {
                acceleration = (Input.GetAxis("PS4 R2") * 0.5f + 0.5f) - (Input.GetAxis("PS4 L2") * 0.5f + 0.5f),
                turning = Input.GetAxis("PS4 Horizontal"),
                breaking = Input.GetButton("PS4 Square")
            };
        }
        else
        {
            controller.input = new CarControllerInput
            {
                acceleration = Input.GetAxis("PC Vertical"),
                turning = Input.GetAxis("PC Horizontal"),
                breaking = Input.GetButton("PC Space")
            };
        }
        
    }
}
