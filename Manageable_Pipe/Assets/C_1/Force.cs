using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public bool useForce = false;
    public Rigidbody attractObject;
    public float forceValue = 1f;

    private Vector3 forceDirection;
    private Rigidbody curAttractObject;
    private Rope_tube4 rope_tube;

	void Update ()
    {
		if( attractObject != null )
        {
            if(attractObject != curAttractObject)
            {
                curAttractObject = attractObject;
                GameObject shar = attractObject.gameObject;
                GameObject tube = shar.transform.parent.gameObject;
                rope_tube = tube.GetComponent< Rope_tube4 > ();
                rope_tube.SetCurDragObject(shar);
            }
            if( useForce )
            { 
                forceDirection = gameObject.transform.position - curAttractObject.gameObject.transform.position;
                forceDirection.Normalize();
                curAttractObject.AddForce(forceDirection* forceValue, ForceMode.Impulse );
            }
        }
        else
        {
            if(curAttractObject != null)
            {
                rope_tube.SetCurDragObject(null);
                curAttractObject = null;
            }
        }
    }
}
