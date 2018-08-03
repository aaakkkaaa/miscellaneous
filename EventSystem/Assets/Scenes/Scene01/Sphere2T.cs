using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere2T : MonoBehaviour
{

    public static event EventController.MethodContainer OnAbroadRight;

    public void TeleportDown(string message, Transform mySenderTransf)
    {
        transform.Translate(Vector3.down);
        print(message + ". Отправил: " + mySenderTransf);
        if (OnAbroadRight != null)
        {
            if (transform.position.y < -2) OnAbroadRight("Правый шар вышел!", transform);
        }
    }

    public void ResetPosit(string message, Transform mySenderTransf)
    {
        transform.position = new Vector3(2, 1, 0);
        print(message + ". Отправил: " + mySenderTransf);
    }

}
