using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScriptHerder : MonoBehaviour
{
    private WorldController _worldController;
    // содержит список action данного шага
    private ScriptData _scriptData;
    // Делегат - для получения событий от Control
    public delegate void MyEvent(string NativePath, Transform mySenderTransf);

    void Awake()
    {
        _worldController = GetComponent<WorldController>();
        Control.MyStateChanged += OnStateChanged;
    }

    public void Load(string scriptFName, string worldFName )
    {
        _scriptData = ScriptData.Load( Path.Combine(Application.dataPath, scriptFName) );
        _worldController.Load( worldFName );
    }

    // обработчик событий
    public void OnStateChanged(string NativePath, Transform mySenderTransf)
    {
        print("Обработчик: OnStateChanged" + ", Полное имя объекта в иерархии сцены: " + NativePath + ", Публикатор: " + mySenderTransf);
        print(mySenderTransf.position.ToString("F4"));
        State state = mySenderTransf.gameObject.GetComponent<Control>().GetState();
    }

}
