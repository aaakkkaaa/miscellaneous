using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGlobals : MonoBehaviour
{

    // Глобальные свойства
    public string Headset { get; }
    public Transform WhereToLook { get; set; }

    // Делегат - для формирования событий
    public delegate void MyEvent(string NativePath, Transform mySenderTransf);


    // Use this for initialization
    void Start()
    {

        // Подпишемся на события
        Sender.MyStateChanged += OnStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Обработчик события OnStateChanged
    public void OnStateChanged(string NativePath, Transform mySenderTransf)
    {
        print("Обработчик: OnStateChanged" + ", Полное имя объекта в иерархии сцены: " + NativePath + ", Публикатор: " + mySenderTransf);
        print(mySenderTransf.position.ToString("F4"));
    }
}