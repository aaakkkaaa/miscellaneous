using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    protected GameObject myCap1, myCap2;

    // Use this for initialization
    void Start () {
        GameObject.Find("DropRed").GetComponent<Renderer>().material.color = new Color(1.0f, 0f, 0.0f, 0.7f);
        GameObject.Find("DropBlue").GetComponent<Renderer>().material.color = new Color(0.0f, 0f, 1.0f, 0.7f);
//        GameObject.Find("DropViolet1").GetComponent<Renderer>().material.color = new Color(1.0f, 0f, 1.0f, 0.7f);
//        GameObject.Find("DropViolet2").GetComponent<Renderer>().material.color = new Color(1.0f, 0f, 1.0f, 0.7f);

        
//        myCap1 = GameObject.Find("Cap1");
        myCap1 = GameObject.Find("Cargo_CTB_Full_Size.001");
        
        if (myCap1 != null)
        {
            float x0 = myCap1.transform.localScale.x / 2;
            myCap1.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0);
        }

        myCap2 = GameObject.Find("Cap");

        if (myCap2 != null)
        {
//            float x0 = myCap2.transform.localScale.x / 2;
//            myCap2.GetComponent<Rigidbody>().centerOfMass = new Vector3(-x0, 0, 0);
        }


    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown("1"))
        {
            print("button 1 down");
            
            string[] myArray = UnityEngine.Input.GetJoystickNames();
            print("myArray.Length="+myArray.Length);
            for (int i=0;i<myArray.Length;i++)
            {
                print(myArray[i]);
            }
        }
	}
}

/*
 Oculus Touch Controller - Left
 Oculus Touch Controller - Right
 OpenVR Controller(Oculus Rift CV1 (Left Controller)) - Left
 OpenVR Controller(Oculus Rift CV1 (Right Controller)) - Right
 */
