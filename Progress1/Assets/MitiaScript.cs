using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitiaScript : MonoBehaviour, IInteractive
{
    private Control _control;

    private string _openState = "close";
    private float _param;

    private GameObject _cover;

    private void Awake()
    {
        // Наладить связь с контролом
        _control = gameObject.GetComponent<Control>();
        _control.SetInteractive(this);

        // это для примера, внутренняя структура:
        _cover = transform.Find("cover").gameObject;
        print("_cover = " + _cover);
    }

    void Update ()
    {
        string newOpenState = _openState;

        //определение степени открытости крышки
        Vector3 rot = _cover.transform.localEulerAngles;
        print(rot.x);
        /*
        if (rot.x >= 0)
        {
            rot.x = 0;
            _cover.transform.localEulerAngles = rot;
        }
        if (rot.x < -120)
        {
            rot.x = -120;
            _cover.transform.localEulerAngles = rot;
        }
        */
        if (rot.x >= -10 )
        {
            newOpenState = "close";
        }
        else if(rot.x <= -110)
        {
            newOpenState = "open";
        }
        else
        {
            newOpenState = "open";
        }
        if(newOpenState != _openState )
        {
            _openState = newOpenState;
            _param = (rot.x - (-120)) / (0 - (-120)) * 100;
            _control.ChangeState();
        }
    }

    public State getState()
    {
        State state = new State("free", _openState, _param);
        // заполнить нужные поля State
        return state;
    }

    public void setState(State s)
    {
        
    }

}
