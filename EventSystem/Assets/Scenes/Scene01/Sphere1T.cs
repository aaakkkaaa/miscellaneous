using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere1T : MonoBehaviour {

    public static event EventController.MethodContainer OnAbroadLeft;

    public void TeleportUp(string message, Transform mySenderTransf)
    {
        transform.Translate(Vector3.up);
        print(message + ". Отправил: " + mySenderTransf);
        if (transform.position.y > 4) OnAbroadLeft("Левый шар вышел!", transform);
    }

    public void ResetPosit(string message, Transform mySenderTransf)
    {
        transform.position = new Vector3(-2, 1, 0);
        print(message + ". Отправил: " + mySenderTransf);
    }

}
