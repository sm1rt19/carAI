﻿using System.Collections;
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
            vertical = Input.GetAxis("Vertical"),
            horizontal = Input.GetAxis("Horizontal"),
            breaking = Input.GetKey(KeyCode.Space)
        };
    }
}
