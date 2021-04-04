using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_control3d : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 moveSpeed;

    private void Update()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            rb.AddForce(moveSpeed);
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            rb.AddForce(-moveSpeed);
        }
    }
}
