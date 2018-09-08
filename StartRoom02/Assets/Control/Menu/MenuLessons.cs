using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Linq;


public class MenuLessons : MonoBehaviour
{
    private MainMenu _mainMenu;
    private Dictionary<string, XDocument> _dic;
    // сами меню
    private GameObject _menuLes;
    private GameObject _menuParts;
    private GameObject _menuSteps;

    // для расположения кнопок
    private GameObject _lesContent;
    private GameObject _partContent;
    private GameObject _stepContent;

    // кнопка образец для всех кнопок меню
    private GameObject _shablonBtn;

    // список кнопок в меню part
    private List<MenuLesBtn> _partBtnList;
    // список кнопок в меню step
    private List<MenuLesBtn> _stepBtnList;

    // номера выбранных пунктров меню
    private string _lesNum;
    private string _partNum;
    // названия выбранных пунктов
    private string _lesName;
    private string _partName;
    private string _stepName;

    private void Awake()
    {
        _mainMenu = GameObject.Find("MenuCanvas").GetComponent<MainMenu>();
        _menuLes = GameObject.Find("AllMenuLesson/LessonView");
        _menuParts = GameObject.Find("AllMenuLesson/PartsView");
        _menuSteps = GameObject.Find("AllMenuLesson/StepView");
        // прототип кнопки
        _shablonBtn = GameObject.Find("LessonView/Viewport/Content/LesMenuBtn");
        // куда будем ставить кнопки с названиями тем
        _lesContent = GameObject.Find("AllMenuLesson/LessonView/Viewport/Content");
        _partContent = GameObject.Find("AllMenuLesson/PartsView/Viewport/Content");
        _stepContent = GameObject.Find("AllMenuLesson/StepView/Viewport/Content");


        _menuParts.SetActive(false);
        _menuSteps.SetActive(false);
        _shablonBtn.SetActive(false);
    }

    public void Show()
    {
        _menuLes.SetActive(true);
    }

    // меню первого уровня (Темы)
    public void CreateLessonsMenu( Dictionary<string, XDocument> dic)
    {
        _dic = dic;
        // строим меню первого уровня
        _shablonBtn.SetActive(true);
        print("content = "+ _lesContent);
        // установим правильный размер content
        int len = dic.Count;
        RectTransform rectTransf = _lesContent.GetComponent<RectTransform>();
        Vector2 offMin = rectTransf.offsetMin;
        offMin.y = -(20 + 30 * len);
        rectTransf.offsetMin = offMin;

        // расстановка кнопок
        int numKeys = 0;
        foreach (string les in dic.Keys)
        {
            GameObject btn = Instantiate(_shablonBtn);
            btn.name = "les_"+les;
            btn.GetComponent<MenuLesBtn>().Menu = this;    // у кнопки ссылка на меню для вызова функции SelectPerson

            btn.transform.SetParent(_lesContent.transform, false);
            Vector3 pos = btn.transform.localPosition;
            pos.x = 90;
            pos.y = -20 - numKeys * 30;
            btn.transform.localPosition = pos;

            XDocument xml = dic[les];
            string lesName = xml.Root.Attribute("name").Value;
            btn.GetComponent<MenuLesBtn>().SetDate(les, lesName, "les");
            numKeys++;
        }
        // спрячем кнопку - прототип
        _shablonBtn.SetActive(false);

    }

    // Меню второго уровня (Part)
    private void CreatePartsMenu(string num)
    {
        _menuParts.SetActive(true);
        _shablonBtn.SetActive(true);
        RectTransform rectTransf = _partContent.GetComponent<RectTransform>();
        // очистим от предыдущего меню
        if (_partBtnList != null && _partBtnList.Count > 0)
        {
            foreach (MenuLesBtn btn in _partBtnList)
            {
                //btn.transform.parent = null;
                btn.transform.SetParent(null);
                Destroy(btn.gameObject);
            }
        }
        _partBtnList = new List<MenuLesBtn>();

        // Подготовим данные для создания меню разделов Part
        XDocument xmlLes = _dic[num];
        IEnumerable<XElement> myParts = xmlLes.Root.Elements("part");
        int count = 0;
        foreach(XElement part in myParts)
        {
            GameObject btn = Instantiate(_shablonBtn);
            btn.name = "part_" + count;
            btn.GetComponent<MenuLesBtn>().Menu = this;
            string partName = part.Attribute("name").Value;
            string partNum = part.Attribute("num").Value;
            btn.GetComponent<MenuLesBtn>().SetDate(partNum, partName, "part");
            _partBtnList.Add(btn.GetComponent<MenuLesBtn>());
            count++;
        }
        // подготовим место под кнопки
        Vector2 offMin = rectTransf.offsetMin;
        offMin.y = -(20 + 30 * count);
        rectTransf.offsetMin = offMin;
        // поставим кнопки
        for(int i=0; i< _partBtnList.Count; ++i)
        {
            GameObject btn = _partBtnList[i].gameObject;
            //btn.transform.SetParent(_partContent.transform, false);
            btn.transform.SetParent(rectTransf, false);
            Vector3 pos = btn.transform.localPosition;
            pos.x = 90;
            pos.y = -20 - i * 30;
            btn.transform.localPosition = pos;
        }
        _shablonBtn.SetActive(false);
    }

    // Меню третьего уровня (Step)
    private void CreateStepsMenu(string num)
    {
        _menuSteps.SetActive(true);
        _shablonBtn.SetActive(true);
        RectTransform rectTransf = _stepContent.GetComponent<RectTransform>();
        // очистим от предыдущего меню
        if (_stepBtnList != null && _stepBtnList.Count > 0)
        {
            foreach (MenuLesBtn btn in _stepBtnList)
            {
                //btn.transform.parent = null;
                btn.transform.SetParent(null);
                Destroy(btn.gameObject);
            }
        }
        _stepBtnList = new List<MenuLesBtn>();

        // Подготовим данные для создания меню разделов Part
        XDocument xmlLes = _dic[_lesNum];
        IEnumerable<XElement> myParts = from myEl in xmlLes.Root.Elements("part") where (string)myEl.Attribute("num") == _partNum select myEl;
        XElement curPart = myParts.First(); // берем только первый (он же должен быть единственный)
        IEnumerable<XElement> mySteps = curPart.Elements("step");

        int count = 0;
        foreach (XElement step in mySteps)
        {
            print(step);
            GameObject btn = Instantiate(_shablonBtn);
            btn.name = "step_" + count;
            btn.GetComponent<MenuLesBtn>().Menu = this;
            string stepName = step.Attribute("name").Value;
            string stepNum = step.Attribute("num").Value;
            btn.GetComponent<MenuLesBtn>().SetDate(stepNum, stepName, "step");
            _stepBtnList.Add(btn.GetComponent<MenuLesBtn>());
            count++;
        }
        // подготовим место под кнопки
        Vector2 offMin = rectTransf.offsetMin;
        offMin.y = -(20 + 30 * count);
        rectTransf.offsetMin = offMin;
        // поставим кнопки
        for (int i = 0; i < _stepBtnList.Count; ++i)
        {
            GameObject btn = _stepBtnList[i].gameObject;
            //btn.transform.SetParent(_partContent.transform, false);
            btn.transform.SetParent(rectTransf, false);
            Vector3 pos = btn.transform.localPosition;
            pos.x = 90;
            pos.y = -20 - i * 30;
            btn.transform.localPosition = pos;
        }
        _shablonBtn.SetActive(false);
    }


    // вызывается кнопкой меню
    public void SelectPunct(string menuType, string txtNum)
    {
        print("SelectPunct() "+ menuType +" " + txtNum);
        if (menuType == "les")
        {
            RectTransform rectTransf = _lesContent.GetComponent<RectTransform>();
            for (int i = 0; i < rectTransf.childCount; ++i)
            {
                GameObject btnObj = rectTransf.GetChild(i).gameObject;
                MenuLesBtn btn = btnObj.GetComponent<MenuLesBtn>();
                if (btn.BtnNum != txtNum)
                {
                    btn.SetNorm();
                }
                else
                {
                    _lesName = btn.BtnText;
                }
            }
            _menuSteps.SetActive(false);
            CreatePartsMenu(txtNum);
            _lesNum = txtNum;
        }
        else if(menuType == "part")
        {
            RectTransform rectTransf = _partContent.GetComponent<RectTransform>();
            for (int i = 0; i < rectTransf.childCount; ++i)
            {
                GameObject btnObj = rectTransf.GetChild(i).gameObject;
                MenuLesBtn btn = btnObj.GetComponent<MenuLesBtn>();
                if (btn.BtnNum != txtNum)
                {
                    btn.SetNorm();
                }
                else
                {
                    _partName = btn.BtnText;
                }
            }
            _partNum = txtNum;
            CreateStepsMenu(txtNum);
        }
        else if (menuType == "step")
        {
            RectTransform rectTransf = _stepContent.GetComponent<RectTransform>();
            for (int i = 0; i < rectTransf.childCount; ++i)
            {
                GameObject btnObj = rectTransf.GetChild(i).gameObject;
                MenuLesBtn btn = btnObj.GetComponent<MenuLesBtn>();
                if (btn.BtnNum != txtNum)
                {
                    btn.SetNorm();
                }
                else
                {
                    _stepName = btn.BtnText; 
                }
            }
            LesMenuDeselect();
            _menuLes.SetActive(false);
            _menuParts.SetActive(false);
            _menuSteps.SetActive(false);

            _mainMenu.SetLessonString( _lesName + "->" + _partName + "->" + _stepName);
            _mainMenu.StartLesson(_lesNum, _partNum, txtNum);
        }
    }

    private void LesMenuDeselect()
    {
        RectTransform rectTransf = _lesContent.GetComponent<RectTransform>();
        for (int i = 0; i < rectTransf.childCount; ++i)
        {
            GameObject btnObj = rectTransf.GetChild(i).gameObject;
            MenuLesBtn btn = btnObj.GetComponent<MenuLesBtn>();
            btn.SetNorm();
        }
    }


}
