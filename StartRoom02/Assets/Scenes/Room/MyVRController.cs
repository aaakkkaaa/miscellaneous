using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVRController : MonoBehaviour, IInteractive
{
    private Control _control;

    private void Awake()
    {
        // Наладить связь с контролом
        _control = gameObject.GetComponent<Control>();
        _control.SetInteractive(this);

    }

    // ************* Реализация функций интерфейса IInteractive ************************

    // запрашивается Контролом
    public State getState()
    {
        return null;
    }

    // Вызывается из Контрола, например при загрузке мира или настройке параметров <action> сценария
    public void setState(State s)
    {
        
    }
}
