﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGlobals : MonoBehaviour
{

    // Глобальные свойства
    public string GlHeadset { get; }
    public Transform GlWhatToSeeTr { get; set; }

    // Настройки
    public bool isTips = true;
    public bool isInstruc = true;
    public float sndVolume = 1.0f;      // TODO: уточнить от 0..1 или 0..100?
    public bool isJoystick = true;

    // Объект для вывода текстовых сообщений в пространство
    UI_TextMessage ui_Message;

    // Для построения пути к файлам xml
    public string myDataPath = "XML_Data";
    // Имя подпапки для файлов описания world. Когда сделаем учеников, будем хранить в своей папке для каждого
    public string myXWorldPath = "WorldShots";
    // Имя подпапки для файлов уроков. Перенести в общеизвестное место и оформить, как паблик?
    public string myXFilePath = "Lessons";


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