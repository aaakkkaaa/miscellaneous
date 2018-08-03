using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {

    public delegate void MethodContainer(string message, Transform mySenderTransf);
    //public delegate void EventHandler

    public Sphere1T s1t;
    public Sphere2T s2t;

    void Awake()
    {
        InputAggregator.OnTeleportEvent += s1t.TeleportUp; //Обратите внимание, линк на класс (скрипт), содержащий событие, делать не нужно!
        InputAggregator.OnTeleportEvent += s2t.TeleportDown;

        Sphere1T.OnAbroadLeft += s1t.ResetPosit;
        Sphere2T.OnAbroadRight += s2t.ResetPosit;
    }

}
