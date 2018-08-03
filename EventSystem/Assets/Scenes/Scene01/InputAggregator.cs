using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAggregator : MonoBehaviour {

    public static event EventController.MethodContainer OnTeleportEvent;

    private void Start()
    {
        InputAggregator myThis = this;
        print("myThis.name = " + myThis.name + ", this = " + this);
    }

    void Update()
    {
        if (Input.GetKeyDown("space")) OnTeleportEvent("Телепорт Эвент!", transform);

    }
}
