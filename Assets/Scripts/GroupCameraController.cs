using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupCameraController : MonoBehaviour
{
    public float stiffness = 0.3f;
    public Vector3 PositionOffset;
    private GameObject[] cars; 

    void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Car AI");
    }

    void Update()
    {
        var activeCars = cars.Where(x => x.activeSelf).ToList();
        if (activeCars.Any())
        {
            float dx = activeCars.Average(x => x.transform.position.x);
            float dz = activeCars.Average(x => x.transform.position.z);
            var target = new Vector3(dx, 0, dz) + PositionOffset;
            transform.position = Vector3.MoveTowards(transform.position, target, stiffness);
        }
    }
}
