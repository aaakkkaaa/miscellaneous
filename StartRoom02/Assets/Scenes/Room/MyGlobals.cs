using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGlobals : MonoBehaviour
{

    // Глобальные свойства
    public string GlHeadset { get; }
    public Transform GlWhatToSeeTr { get; set; }

    // Объект для вывода текстовых сообщений в пространство
    UI_TextMessage ui_Message;
    
    // Use this for initialization
    void Start()
    {
        // Объект для вывода текстовых сообщений в 3D пространство
        ui_Message = GameObject.Find("Message").GetComponent<UI_TextMessage>();

    }


    // ========================= Публичные методы =====================================

    // Подготовить и показать текстовое сообщение.
    public void GlShowMessage(string myMessage, float myLifeTime)
    {
        ui_Message.MyFuncShowMessage(myMessage, myLifeTime);
    }

    // Подготовить и показать текстовое сообщение.
    // Перегруженный метод (см. выше) - если не указано время жизни сообщения, тогда 3 секунды
    public void GlShowMessage(string myMessage)
    {
        ui_Message.MyFuncShowMessage(myMessage, 3.0f);
    }

}