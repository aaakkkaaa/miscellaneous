using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitiaScript : MonoBehaviour, IInteractive
{
    private Control _control;

    private string _openState = "close";
    private float _param = -1;

    private GameObject _cover;

    private void Awake()
    {
        // Наладить связь с контролом
        _control = gameObject.GetComponent<Control>();
        _control.SetInteractive(this);

        // это для примера, внутренняя структура:
        _cover = transform.Find("cover").gameObject;
        //print("_cover = " + _cover);
    }

    void Update ()
    {
        string newOpenState = _openState;

        //определение степени открытости крышки
        Vector3 rot = _cover.transform.localEulerAngles;
       // print("rot.x = "+ rot.x);

        if (rot.x >= 350 )
        {
            newOpenState = "close";
        }
        //if (rot.x <= 280 && rot.x > 0)
        if (rot.x <= 280)
        {
            newOpenState = "open";
        }
        //print("newOpenState = " + newOpenState);
        if(newOpenState != _openState )
        {
            _openState = newOpenState;
            _param = (90 - (360-rot.x) ) / (90) * 100;
            if( _control != null )
            { 
                _control.ChangeState();
            }
        }
    }

    // ************* Реализация функций интерфейса IInteractive ************************

    // запрашивается Контролом
    public State getState()
    {
        // создать объект для передачи состояний
        State state = new State("free", _openState, _param);
        // заполнить нужные поля State

        // вернуть
        return state;
    }

    // Вызывается из Контрола, например при загрузке мира
    public void setState(State s)
    {
        // привести положение и др. свойства в соответствие с переданным состоянием



    }

}
