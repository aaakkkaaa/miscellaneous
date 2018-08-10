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

        // Примеры вызовов, которые будут использоваться в процессе анализа предикторов
        // Как получить значение предиктора вида [room/box.openState=open]
        // Расчленяем на строки и загоняем их в переменные:
        // string myPath (сюда пойдет "room/box")
        // string propName (сюда пойдет "openState")
        // string propValue (сюда пойдет "open")
        // получить Control:
        // Control ctrl = _worldController.SourceControls[myPath];
        // Потом у контрола вызвать функцию:
        // bool result = ctrl.GetState(propName, propValue);

        // Если нужно проверять вхождение одного объекта имеющего компонент Control в другой, имеющий компонент Control
        // Оба контролы, так как в предикторах используются NativePath
        // [room/box.parent=room/bigbox1]
        // Control ctrl = _worldController.SourceControls["room/box"];
        // bool result = ctrl.CheckParent( "room/bigbox1" )

    }

}
