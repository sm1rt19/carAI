using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PlayerClient : MonoBehaviour
{
    public CarControllerBase CarController;


    void Update()
    {
        var ps4 = false;
        if (ps4)
        {
            CarController.controllerInput = new CarControllerInput
            {
                vertical = (Input.GetAxis("PS4 R2") * 0.5f + 0.5f) - (Input.GetAxis("PS4 L2") * 0.5f + 0.5f),
                horizontal = Input.GetAxis("PS4 Horizontal"),
                breaking = Input.GetButton("PS4 Square")
            };
        }
        else
        {
            CarController.controllerInput = new CarControllerInput
            {
                vertical = Input.GetAxis("PC Vertical"),
                horizontal = Input.GetAxis("PC Horizontal"),
                breaking = Input.GetButton("PC Space")
            };
        }
        
    }
}
