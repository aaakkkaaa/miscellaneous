using UnityEngine;

public class Control : MonoBehaviour
{
    protected ControlData _controlData;             // хранение, загрузка, выгрузка данных о состоянии этого Control
    private WorldController _worldController;       // скрипт описания мира
    private IInteractive _inter;                    // скрипт бизнес логики, привязанные к тому же gameObject

    private string _nativePath;
    public string NativePath
    {
        get { return _nativePath; }
        set { _nativePath = value; }
    }

    public WorldController worldController
    {
        set
        {
            _worldController = value;
        }
    }

    public static event ScriptHerder.MyEvent MyStateChanged;

    
    public string CreatePath()
    {
        Transform curTransform = transform;
        string p = this.name;

        while (curTransform.parent != null)
        {
            p = curTransform.parent.gameObject.name + "/" + p;
            curTransform = curTransform.parent;
        }
        return p;
    }
    

    public virtual void Init(ControlData cd)
    {
        _controlData = cd;
        gameObject.SetActive(_controlData.active);
        // координаты, углы, масштаб
        gameObject.transform.localPosition = _controlData.GetPos();
        gameObject.transform.localEulerAngles = _controlData.GetRot();
        gameObject.transform.localScale = _controlData.GetScale();

        // передать данные для инициализации объекту IInteractive
        if (_controlData.state == null)
        {
            print(cd.nativePath + "   cd.state = null");
        }
        else
        {
            if(_inter != null)
            {
                _inter.setState(_controlData.state);
            }
        }
    }

    public void SetState( State st)
    {
        // надо ли продублировать изменения в _controlData.state?
        if (_inter != null)
        {
            _inter.setState(st);
        }
    }

    public virtual ControlData PrepareDataToSave()
    {
        if (_controlData == null)
        {
            _controlData = new ControlData();
        }
        // Запомнить пути
        _controlData.nativePath = _nativePath;
        _controlData.currentPath = CreatePath();
        print("_controlData.nativePath "+ _controlData.nativePath);

        // координаты, углы, масштаб
        _controlData.SetPos(gameObject.transform.localPosition);
        _controlData.SetRot(gameObject.transform.localEulerAngles);
        _controlData.SetScale(gameObject.transform.localScale);

        // получаем от скрипта IInteractive конкретного объекта параметры состояния:
        // freeState: "fixed", "free", "hand_r", hand_l",  ""-это значит не используется
        // openState: "close", "ajar", "open",  ""-это значит не используется
        // param
        // и записать в _controlData.state
        if (_inter != null)
        {
            State state = _inter.getState();
            _controlData.state = state;
        }
        _controlData.active = gameObject.activeSelf;
        return _controlData;
    }

    // ***************************** Взаимодействие с бизнес-логикой **************************************

    // Митин скрипт передает ссылку на себя в начале работы 
    public void SetInteractive(IInteractive inter)
    {
        _inter = inter;
    }

    // Митин объект изменил свое состояние, нужно его запросить
    public void ChangeState()
    {
        if (_controlData == null)
        {
            _controlData = new ControlData();
        }
        _controlData.state = _inter.getState();

        // посылка сообщений в сценарий через вызов делегата
        MyStateChanged(_nativePath, transform);

    }

    // *************************** Взаимодействие со сценарием *********************************************

    // возвращает полное состояние контрола
    public State GetState()
    {
        return _controlData.state;  
    }

    // возвращает значение конкретного состояния
    public string GetState(string property)
    {
        if(_controlData.state == null)
        {
            return "";
        }
        switch (property)       // если список будет увеличиваться, может сделать словарь?
        {
            case "freeState":
                return _controlData.state.freeState;
            case "openState":
                return _controlData.state.openState;
            case "downState":
                return _controlData.state.downState;  // down или up
            default:
                return "";
        }
    }

    // возвращает значение конкретного состояния
    public bool GetState(string property, string value)
    {
        return (value == GetState(property)); 
    }

    // если в сценарии есть раздел <commands><object><state>.....
    public void SetState(string property, string value)
    {
        if (_controlData.state == null)
        {
            _controlData.state = new State();
        }
        switch (property)       
        {
            case "freeState":
                    _controlData.state.freeState = value;
                     break;
            case "openState":
                     _controlData.state.openState = value;
                     break;
            case "downState":
                     _controlData.state.downState = value;
                     break;
        }
        SetState(_controlData.state);       // там передаем state в _inter
    }
    public void SetState(string property, float value)
    {
        if (_controlData.state == null)
        {
            _controlData.state = new State();
        }
        switch (property)       
        {
            case "param":
                _controlData.state.param = value;
                break;
        }
        SetState(_controlData.state);       // там передаем state в _inter
    }

    // По переданному NativePath проверяем, является ли он родителем данного объекта?
    public bool CheckParent( string anyPath )
    {
        // TODO: возможно, надо искать объект не в словаре, так как он не обязательно контрол?
        Control candidat = _worldController.SourceControls[anyPath];
        return (transform.parent.gameObject == candidat.gameObject);
    }

}