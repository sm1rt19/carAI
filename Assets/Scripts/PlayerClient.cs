using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClient : MonoBehaviour
{
    private Car car;

    void Start()
    {
        car = GetComponent<Car>();
    }

    void LateUpdate()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        if (dx != 0)
            car.Turn(dx);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            car.Accelerate();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            car.Break();
        }
    }
}
