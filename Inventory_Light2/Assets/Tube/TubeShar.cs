using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeShar : MonoBehaviour
{
    public bool isCollision;
    public int Index;
    public Vector3 ColliderImpulse;
    public bool isFixed;
    // количество коллизий для данного объекта
    private int _numCollision;

    private void OnCollisionEnter(Collision collision)
    {
        //print(Index + " коллизия!");
        _numCollision++;
        isCollision = true;
        SaveCollisionDate(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        SaveCollisionDate(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        _numCollision--;
        if(_numCollision == 0)
        {
            isCollision = false;
        }
    }

    private void SaveCollisionDate(Collision collision)
    {
        ColliderImpulse = collision.impulse * Time.fixedDeltaTime;
    }
}
