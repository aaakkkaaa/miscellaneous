using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    // путь к контролу на момент старта -> Control
    private Dictionary<string, Control> _sourceControls;
    // содержит список настроек контролов, считанных из XML
    public WorldData worldData;
    // имя корневого объекта
    private string rootObjName = "room";

    // Для сценария, если нужен доступ к контролу по "полному имени"
    public Dictionary<string, Control> SourceControls
    {
        get
        {
            return _sourceControls;
        }
    }

    private bool showGUI = false;       // показывать или скрывать интерфейс для сохранения/загрузки


    void Awake()
    {
        // Здесь room - родитель в котором должны быть все контролы
        GameObject room = GameObject.Find(rootObjName);  
        Control[] controls = room.GetComponentsInChildren<Control>();
        print("controls.Count " + controls.Length);

        // строим словарь "путь по состоянию на момент запуска" -> Control 
        _sourceControls = new Dictionary<string, Control>();
        foreach (Control ctrl in controls)
        {
            print(ctrl.NativePath);
            _sourceControls.Add(ctrl.NativePath, ctrl);
        }
    }

    private void Update()
    {
        if ( Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown("s"))
        {
            showGUI = !showGUI;
        }

    }

    public void Load(string name)
    {
        worldData = WorldData.Load(Path.Combine(Application.dataPath, name));
        Control ctrl;

        // разложить все Control по правильным местам в иерархии
        for (int i = 0; i < worldData.controlsData.Count; ++i)
        {
            // проверим, что объект не на месте 
            string strWork = worldData.controlsData[i].nativePath;  
            ctrl = _sourceControls[strWork];
            string strCur = ctrl.CreatePath();
            if(strCur != worldData.controlsData[i].currentPath)
            {
                print("Надо перемещать");
                GameObject workObject = ctrl.gameObject;
                string strParent = worldData.controlsData[i].parentPath;
                GameObject workParent = _sourceControls[strParent].gameObject;
                workObject.transform.parent = workParent.transform;
            }
        }
        // инициализация
        for (int i = 0; i < worldData.controlsData.Count; ++i)
        {
            ControlData cd = worldData.controlsData[i];
            //print("cd.nativePath = " + cd.nativePath);
            ctrl = _sourceControls[cd.nativePath];
            print(i);
            ctrl.Init(cd);
        }
    }


    public void Save(string name)
    {
        // получить список контролов на момент сохранения
        GameObject room = GameObject.Find(rootObjName);
        Control[] controls = room.GetComponentsInChildren<Control>();
        // создать список для сохранения данных каждого Control
        worldData = new WorldData();
        worldData.controlsData = new List<ControlData>();
        for(int i=0; i<controls.Length; ++i)
        {
            print(i);
            ControlData cd = controls[i].PrepareDataToSave();
            worldData.controlsData.Add(cd);
        }
        worldData.worldRoot = rootObjName;


        worldData.Save(Path.Combine(Application.dataPath, name));
    }

    // для отладки - интерфейс для загрузки и сохранения в xml
    string _fileName;
    void OnGUI()
    {
        if(showGUI)
        { 
            _fileName = GUI.TextField(new Rect(10, 10, 100, 20), _fileName);
            if (GUI.Button(new Rect(10, 40, 100, 30), "Load"))
            {
                Load(_fileName);
            }
            if (GUI.Button(new Rect(10, 70, 100, 30), "Save"))
            {
                Save(_fileName);
            }
        }

    }
    


}
