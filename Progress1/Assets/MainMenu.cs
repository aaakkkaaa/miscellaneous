using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private ScriptHerder _scriptHerder;

    void Awake()
    {
        GameObject boss = GameObject.Find("Boss");
        _scriptHerder = boss.GetComponent<ScriptHerder>();
    }
        //наверное, должен быть сперва  выбор пользователя и ввод пароля
        //потом загрузка меню для данного пользователя
        //и формирование графического меню, к пунктам которого привязаны файлы-сценарии


    public void StartLoad()
    {
        print(" ====== MainMenu -> Кнопка Загрузка -> StartLoad() ====== ");
        _scriptHerder.Load("step01.xml", "world1.xml");
    }
}
