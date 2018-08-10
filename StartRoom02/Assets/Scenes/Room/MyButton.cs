using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton : MonoBehaviour, IInteractive {

    private Control _control;

    private string myButtonState;

    private void Awake()
    {
        // Наладить связь с контролом
        _control = gameObject.GetComponent<Control>();
        _control.SetInteractive(this);

    }

    // Update is called once per frame
    void Update ()
    {
        // ПРОВЕРИТЬ! Пока используем стандартный input
        if (Input.GetButtonDown("RightMenuButt"))
        {
            print("Меня нажали!!!!!!!!!!!!!!!!!!!");
        }
        else if (Input.GetButtonUp("RightMenuButt"))
        {
            print("Меня нажали!!!!!!!!!!!!!!!!!!!");
        }
        else if (Input.GetKeyDown("f"))
        {
            myButtonState = "down";
            print("Меня нажали!!!!!!!!!!!!!!!!!!!");
            _control.ChangeState();
        }
        else if (Input.GetKeyUp("f"))
        {
            myButtonState = "up";
            print("Меня отжали!!!!!!!!!!!!!!!!!!!");
            _control.ChangeState();

        }
    }

    // ************* Реализация функций интерфейса IInteractive ************************

    // запрашивается Контролом
    public State getState()
    {
        // создать объект для передачи состояний
        State state = new State();
        // заполнить нужные поля State
        state.downState = myButtonState;

        // вернуть
        return state;
    }

    // Вызывается из Контрола, например при загрузке мира
    public void setState(State s)
    {
        // привести положение и др. свойства в соответствие с переданным состоянием



    }


}
