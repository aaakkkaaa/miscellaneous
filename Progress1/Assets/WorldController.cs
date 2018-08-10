using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldController : MonoBehaviour
{

    // имя корневого объекта
    private string _rootObjName = "room";
    // список всех контролов, создается рекурсивным обходом, используется при загрузке и сохранении
    private List<Control> _controls;
    // Словарь: <Путь_к_контролу_на_момент_старта, Control>
    private Dictionary<string, Control> _sourceControls;

    // содержит список настроек контролов, считанных из XML
    public WorldData worldData;

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
        GameObject room = GameObject.Find(_rootObjName);
        // строим список всех контролов, рекурсивно обходя начиная с room
        _controls = new List<Control>();
        CreateControlList(room.transform);
        print("controls.Count " + _controls.Count);

        // строим словарь "путь по состоянию на момент запуска" -> Control 
        _sourceControls = new Dictionary<string, Control>();
        foreach (Control ctrl in _controls)
        {
            ctrl.NativePath = ctrl.CreatePath();
            ctrl.worldController = this;
            print(ctrl.NativePath);
            _sourceControls.Add(ctrl.NativePath, ctrl);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown("s"))
        {
            showGUI = !showGUI;
        }
    }

    // рекурсивная функция для обхода всех начиная с Parent (включительно) и построения списка контролов
    void CreateControlList( Transform parent )
    {
        Control ctrl = parent.gameObject.GetComponent<Control>();
        if(ctrl != null )
        {
            _controls.Add(ctrl);
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            CreateControlList(parent.GetChild(i));
        }
    }

    // загрузка xml описания мира, активизация всех контролов, расстановка их 
    public void Load(string name)
    {
        Control ctrl;

        worldData = WorldData.Load(Path.Combine(Application.dataPath, name));

        // активировать все объекты из словара _sourceControls
        foreach (Control c in _sourceControls.Values)
        {
            c.gameObject.SetActive(true);
        }

        // разложить все Control по правильным местам в иерархии
        int[] rang = new int[worldData.controlsData.Count]; // для хранения длинн пути
        for (int i = 0; i < worldData.controlsData.Count; ++i)
        {
            rang[i] = worldData.controlsData[i].currentPath.Split( new Char[] { '/' }).Length;
        }
        int pathLength = 1;     // длина пути, будем идти от наименьшей
        int processed = 0;      // сколько обработано
        while(processed < worldData.controlsData.Count)
        {
            for (int i = 0; i < worldData.controlsData.Count; ++i)
            {
                if(rang[i] == pathLength)   
                {
                    // проверим, на месте ли объект
                    string strWork = worldData.controlsData[i].nativePath;
                    ctrl = _sourceControls[strWork];
                    string strCur = ctrl.CreatePath();
                    if (strCur != worldData.controlsData[i].currentPath)
                    {
                        print("Надо перемещать");
                        int lastSlesh = worldData.controlsData[i].currentPath.LastIndexOf('/');
                        string strParent = worldData.controlsData[i].currentPath.Remove(lastSlesh);
                        GameObject curParent = GameObject.Find(strParent); // ищем родителя по полному пути
                        if(curParent != null)
                        {
                            ctrl.gameObject.transform.parent = curParent.transform;
                        }
                    }
                    processed++;
                }
            }
            pathLength++;
        }

/*
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

                GameObject workParent=null;
                // это работает только если parent тоже Control и имеется в словаре. Обязательно ли это соблюдается?
                try
                { 
                    workParent = _sourceControls[strParent].gameObject;
                }
                catch (KeyNotFoundException)
                {
                    print(strWork + " -> Родитель не является контролом " + strParent);
                }
                if (workParent != null)
                {
                    workObject.transform.parent = workParent.transform; // перекладываем
                }
                else
                { 
                    workParent = GameObject.Find(strParent); // ищем родителя по полному пути
                    // TODO: Что делать, если родитель - не контрол, не понятно пока
                    if (workParent != null)
                    {
                        workObject.transform.parent = workParent.transform; // перекладываем
                    }
                    else
                    {
                        print(strWork + " -> Не нашли родителя по его пути: " + strParent);
                    }
                }
            }
        }
*/

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
        // создать список для сохранения данных каждого Control
        worldData = new WorldData();
        worldData.controlsData = new List<ControlData>();
        for(int i=0; i<_controls.Count; ++i)
        {
            print(i);
            ControlData cd = _controls[i].PrepareDataToSave();
            worldData.controlsData.Add(cd);
        }
        worldData.worldRoot = _rootObjName;

        worldData.Save(Path.Combine(Application.dataPath, name));
    }

    // построение пути до корня
    public string CreatePath( GameObject obj )
    {
        Transform curTransform = obj.transform;
        string p = obj.name;

        while (curTransform.parent != null)
        {
            p = curTransform.parent.gameObject.name + "/" + p;
            curTransform = curTransform.parent;
        }
        return p;
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
