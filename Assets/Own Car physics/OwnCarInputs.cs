using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnCarInputs : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public bool breaking;

    protected void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        breaking = Input.GetKey(KeyCode.Space);
    }
}
