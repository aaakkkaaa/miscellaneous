using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColl : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        /*
        Vector3 pos = transform.position;
        pos += contact.normal;
        transform.position = pos;
        */
        Vector3 cp = contact.point;
        print("коллизия! " + cp + "Impulse = " + collision.impulse * Time.fixedDeltaTime);

    }

}
