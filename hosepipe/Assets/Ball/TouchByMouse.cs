using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchByMouse : MonoBehaviour
{

    Rigidbody ball_rb;

	// Use this for initialization
	void Start ()
    {
        GameObject ball = GameObject.Find("Ball");
        if (ball != null)
        {
            Debug.Log("Ball is here");
            ball_rb = ball.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(0) )
        {
            if (ball_rb != null)
                ball_rb.velocity = (new Vector3(10,0,0) );
                //ball_rb.AddTorque(new Vector3(0, 0, 10000), ForceMode.Acceleration);
        }

    }
}
