using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour {

    // Заготовим событие
    public static event MyGlobals.MyEvent MyStateChanged;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

        // Публикуем событие
        if (Input.GetKeyDown("1"))
        {
            MyStateChanged("Эвент 1", transform);
        }
    }
}
