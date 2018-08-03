using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTo : MonoBehaviour
{
    public Transform TargetY;
    private Vector3 dirY;

    void Update ()
    {
        if (TargetY != null)
        {
            dirY = (TargetY.position - transform.position).normalized;
            Quaternion q = transform.rotation;
            q.SetFromToRotation(new Vector3(0, 1, 0), dirY);
            transform.rotation = q;
        }
    }
}
