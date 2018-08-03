using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskByMouse : MonoBehaviour
{

    Rigidbody disk_rb;

    // Use this for initialization
    void Start()
    {
        GameObject disk1 = GameObject.Find("Disk1");
        if (disk1 != null)
        {
            Debug.Log("Disk1 is here");
            disk_rb = disk1.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (disk_rb != null)
                disk_rb.velocity = (new Vector3(5, 0, -5));
            //ball_rb.AddTorque(new Vector3(0, 0, 10000), ForceMode.Acceleration);
        }

    }
}
