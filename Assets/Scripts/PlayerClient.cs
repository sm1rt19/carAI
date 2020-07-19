using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClient : MonoBehaviour
{
    private CarControllerBase CarController;

    void Start()
    {
        CarController = GetComponent<CarControllerBase>();
        var startLine = GameObject.FindGameObjectWithTag("Start line");
        transform.position = startLine.transform.position;
        transform.rotation = startLine.transform.rotation;
    }


    void Update()
    {
        CarController.controllerInput = new CarControllerInput
        {
            vertical = Input.GetAxis("PC Vertical"),
            horizontal = Input.GetAxis("PC Horizontal"),
            breaking = Input.GetButton("PC Space")
        };
    }
}
