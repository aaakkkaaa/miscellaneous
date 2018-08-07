using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrHerder : MonoBehaviour {

    // Для работы с инструкциями
    Transform myInstrTr;
    UI_Instructions myInstrUI;

    // Для работы с резюме
    Transform mySummaryTr;
    UI_Summary mySummaryUI;

    // Общие параметры и методы
    MyGlobals myGlobals;

    // Use this for initialization
    void Start () {

        // Для работы с инструкциями
        myInstrTr = GameObject.Find("Instructions").transform;
        myInstrUI = myInstrTr.GetComponent<UI_Instructions>();
        // Для работы с резюме
        mySummaryTr = GameObject.Find("Summary").transform;
        mySummaryUI = mySummaryTr.GetComponent<UI_Summary>();

        // Общие параметры и методы
        myGlobals = GameObject.Find("Boss").GetComponent<MyGlobals>();
    }

    // Update is called once per frame
    void Update () {

        // Отладка
        if (Input.GetKeyDown("i"))
        {
            myInstrUI.MyAccText("Сидоров");
            myInstrUI.MyTopicText("Освоение тренажера");
            myInstrUI.MyPartText("Контроллеры", 10);
            myInstrUI.MyStep(1);
            string[] myIns = { "Текст инструкции.\nТекст инструкции.\nТекст инструкции. Текст инструкции. Текст инструкции." };
            myInstrUI.MyInstr(myIns);

            myInstrUI.Show();
        }
        else if (Input.GetKeyDown("k"))
        {
            myInstrUI.Hide();
        }
        else if (Input.GetKeyDown("r"))
        {
            mySummaryUI.Show();
        }
        else if (Input.GetKeyDown("t"))
        {
            mySummaryUI.Hide();
        }
        else if(Input.GetKeyDown("1"))
        {
            myGlobals.GlShowMessage("Нажато один. Привет, мир!"); // Публикуем событие - вывод сообщения
        }
        else if (Input.GetKeyDown("2"))
        {
            myGlobals.GlShowMessage("Нажато два. Пока, мир!", 1); // Публикуем событие - вывод сообщения
        }
    }
}
