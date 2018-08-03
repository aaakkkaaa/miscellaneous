using UnityEngine;

public class Control : MonoBehaviour
{
    protected ControlData _controlData;             // хранение, загрузка, выгрузка данных о состоянии этого Control
    protected WorldController _worldController;     // !!! скрипт описания мира, возможно он не нужен !!!
    private IInteractive _inter;                    // скрипт бизнес логики

    private string _nativePath;
    public string NativePath
    {
        get { return _nativePath; }
    }

    private void Awake()
    {
        _nativePath = CreatePath();
        //print(name + "   " + _nativePath );
        GameObject wcObject = GameObject.Find("Boss");
        _worldController = wcObject.GetComponent<WorldController>();
    }

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
        // координаты, углы, масштаб
        gameObject.transform.localPosition = _controlData.GetPos();
        gameObject.transform.localEulerAngles = _controlData.GetRot();
        gameObject.transform.localScale = _controlData.GetScale();

        if (_controlData.state == null)
        {
            print(cd.nativePath + "   cd.state = null");
        }
        else
        {
            // TODO передать state в скрипт отвечающий за геометрию и интерактивность:
            // _controlData.state.freeState; _controlData.state.openState; _controlData.state.param; 
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

        GameObject parent;
        if (gameObject.transform.parent == null)
        {
            _controlData.parentPath = "";
            // TODO этой проверки не достаточно, надо проверить, если parent есть, что он Control
        }
        else
        {
            parent = gameObject.transform.parent.gameObject;
            Control parentControl = parent.GetComponent<Control>();
            print("parentControl = " + parentControl);
            _controlData.parentPath = parentControl.NativePath;
        }

        // координаты, углы, масштаб
        _controlData.SetPos(gameObject.transform.localPosition);
        _controlData.SetRot(gameObject.transform.localEulerAngles);
        _controlData.SetScale(gameObject.transform.localScale);

        // TODO получить от скрипта конкретного объекта параметры состояния:
        // freeState: "fixed", "free", "hand", ""-это значит не используется
        // openState: "close", "ajar", "open", ""-это значит не используется
        // param
        // и записать в _controlData.state
        if(_inter != null)
        {
            State state = _inter.getState();
            _controlData.state = state;
        }
        return _controlData;
    }

    // ***************************** Взаимодействие с бизнес-логикой **************************************

    // Митин скрипт передает ссылку на себя 
    public void SetInteractive(IInteractive inter)
    {
        _inter = inter;
    }

    // Митин объект изменил свое состояние, нужно его запросить
    public void ChangeState()
    {
        //State newState = _inter.getState();
        //print(newState.param);
    }

    // *************************** Взаимодействие со сценарием *********************************************

    // Вызывается сценарием
    public State GetState()
    {
        return _controlData.state;  // временно, потом уточнить
    }

}