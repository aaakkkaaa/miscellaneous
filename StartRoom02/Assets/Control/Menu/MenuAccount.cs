using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAccount : MonoBehaviour
{
    private enum AlarmMode {Ok, YesNo};

    private MainMenu _mainMenu;
    private Dictionary<string, MainMenu.Person> _dic;
    private List<MenuBtn> _allBtn;
    private string _curPers="";

    // панель для блокировки меню на время работы с полями ввода и кнопками 
    private GameObject _menuBlock;

    // окно перекрывает все, для вывода сообщений о недопустимости действия и для подтверждения необратимых операций
    private GameObject _alarm;
    private Text _alarmText;
    private Button _alarmBtnOk;
    private Button _alarmBtnYes;
    private Button _alarmBtnNo;

    // текст рядом с верхним меню
    private Text _txtBlack;
    private Text _txtWhite;

    // куда будем ставить кнопки
    private GameObject _content;
    // кнопка образец для всех кнопок меню
    private GameObject _shablonBtn;

    // тесктовые поля с подробной информацией об аккаунте
    private InputField _account;
    private InputField _surname;
    private InputField _name;
    private InputField _patronymic;

    // управление аккаунтом
    private Button _btnAdd;
    private Button _btnChange;
    private Button _btnDelete;
    private Button _btnClean;

    private Button _btnOk;
    private Button _btnCancel;


    private void Awake()
    {
        _mainMenu = GameObject.Find("MenuCanvas").GetComponent<MainMenu>();

        FindAlarmObjects();

        _menuBlock = GameObject.Find("AccountsView/MenuBlock");
        _menuBlock.SetActive(false);

        _txtBlack = GameObject.Find("Accounts/AccNameBlack").GetComponent<Text>();
        _txtWhite = GameObject.Find("Accounts/AccNameWhite").GetComponent<Text>();

        _account = GameObject.Find("AccountsView/InputAccount").GetComponent<InputField>();
        _surname = GameObject.Find("AccountsView/InputSurname").GetComponent<InputField>();
        _name = GameObject.Find("AccountsView/InputName").GetComponent<InputField>();
        _patronymic = GameObject.Find("AccountsView/InputPatronymic").GetComponent<InputField>();
        _txtBlack.text = " ";
        _txtWhite.text = " ";
        _account.text = " ";
        _surname.text = " ";
        _name.text = " ";
        _patronymic.text = " ";

        _btnAdd = GameObject.Find("AccountsView/ButtonAdd").GetComponent<Button>();
        _btnAdd.onClick.AddListener(OnAddClick);
        _btnChange = GameObject.Find("AccountsView/ButtonChange").GetComponent<Button>();
        _btnChange.onClick.AddListener(OnChangeClick);
        _btnDelete = GameObject.Find("AccountsView/ButtonDelete").GetComponent<Button>();
        _btnDelete.onClick.AddListener(OnDeleteClick);
        _btnClean = GameObject.Find("AccountsView/ButtonClean").GetComponent<Button>();
        _btnClean.onClick.AddListener(OnCleanClick);

        _btnOk = GameObject.Find("AccountsView/ButtonOK").GetComponent<Button>();
        _btnOk.gameObject.SetActive(false);

        _btnCancel = GameObject.Find("AccountsView/ButtonCancel").GetComponent<Button>();
        _btnCancel.gameObject.SetActive(false);

        // куда будем ставить кнопки
        _content = GameObject.Find("AccountsView/AccountsScroll/Viewport/Content");
        // прототип кнопки
        _shablonBtn = GameObject.Find("AccountsView/AccountsScroll/Viewport/Content/MenuBtn");
    }

    public void Create( ref Dictionary<string, MainMenu.Person> dic)
    {
        _dic = dic;
        CreateAndSetMenuBtn();
        SelectPerson("");
    }

    private void CreateAndSetMenuBtn()
    {
        // если уже были кнопки, уничтожаем их и чистим список
        if(_allBtn != null && _allBtn.Count >0)        
        {
            foreach (MenuBtn btn in _allBtn)
            {
                //btn.transform.parent = null;
                btn.transform.SetParent(null);
                Destroy(btn);
            }
        }
        _allBtn = new List<MenuBtn>();
        _shablonBtn.SetActive(true);
        // установим правильный размер content
        int len = _dic.Count;
        RectTransform rectTransf = _content.GetComponent<RectTransform>();
        Vector2 offMin = rectTransf.offsetMin;
        offMin.y = -(20 + 30 * len);
        rectTransf.offsetMin = offMin;
        // расстановка кнопок
        int numKeys = 0;
        foreach (string acc in _dic.Keys)
        {
            GameObject tmp = Instantiate(_shablonBtn);
            tmp.name = acc;
            tmp.GetComponent<MenuBtn>().Menu = this;    // у кнопки ссылка на меню для вызова функции SelectPerson
            _allBtn.Add(tmp.GetComponent<MenuBtn>());

            tmp.transform.SetParent(_content.transform, false);
            Vector3 pos = tmp.transform.localPosition;
            pos.x = 90;
            pos.y = -20 - numKeys * 30;
            tmp.transform.localPosition = pos;
            //tmp.GetComponent<MenuBtn>().SetNorm();
            tmp.GetComponent<MenuBtn>().SetText(acc);
            numKeys++;
        }
        // спрячем кнопку - прототип
        _shablonBtn.SetActive(false);
    }

    // вызывается кнопкой меню при ее нажатии или при создании нового
    public void SelectPerson( string pers)
    {
        _curPers = pers;
        // заполним поле в строке верхнего меню
        _txtBlack.text = pers;
        _txtWhite.text = pers;
        // заполним поля сбоку от меню
        if(_dic.ContainsKey(pers) )
        {
            MainMenu.Person curPers = _dic[pers];
            _account.text = pers;
            _surname.text = curPers.Surname;
            _name.text = curPers.Name;
            _patronymic.text = curPers.Patronymic;
        }
        // кнопки меню нажатую сделаем активной, остальные не активными
        foreach ( MenuBtn btn in _allBtn)
        {
            if( btn.BtnText != pers )
            {
                print("SetNorm " + btn.BtnText);
                btn.SetNorm();
            }
            else
            {
                print("SetPress " + btn.BtnText);
                _pressedBtn = btn;
                StartCoroutine("BtnSetPress");
                //btn.SetPress();
            }
        }
    }

    MenuBtn _pressedBtn;
    IEnumerator BtnSetPress()
    {
        yield return new WaitForEndOfFrame();
        _pressedBtn.SetPress();
    }

    // ******************************************************************************************

    // Добавление нового пользователя
    private void OnAddClick()
    {
        AccControlBtnHide();
        _btnOk.onClick.AddListener(OnAddOK);
        _btnCancel.onClick.AddListener(OnAddCancel);

        _account.text = "";
        _surname.text = "";
        _name.text = "";
        _patronymic.text = "";

        SetEditFieldReadOnly(false);
    }

    private void OnAddOK()
    {
        // проверка корректности (имя аккаунта - латинскими)
        if(!IsCorrectString(_account.text))
        {
            ShowAlarmMsg("В названии аккаунта можно использовать только латинские буквы, цифры и знак _", AlarmMode.Ok);
            print("неправильное название аккаунта");
            return;
        }
        // проверка уникальности имени аккаунта
        if( _dic.ContainsKey(_account.text ) )
        {
            ShowAlarmMsg("Название аккаунта должно быть уникальным", AlarmMode.Ok );
            print("Название аккаунта должно быть уникальным");
            return;
        }
        // создание нового аккаунта
        MainMenu.Person newPers = new MainMenu.Person();
        newPers.AccountName = _account.text;
        newPers.Name = _name.text;
        newPers.Surname = _surname.text;
        newPers.Patronymic = _patronymic.text;
        _dic.Add(_account.text, newPers);
        _mainMenu.AddAccount(newPers.AccountName);
        // восстановление состояния кнопок
        AccControlBtnRestore();
        // пересоздание меню
        CreateAndSetMenuBtn();
        // установка текущим пунктом меню новый пункт
        _curPers = newPers.AccountName;
        SetCurPersonDataToViewState();
    }

    private void OnAddCancel()
    {
        // восстановить состояние кнопок
        AccControlBtnRestore();
        // восстановить состояние полей
        if(_curPers == "")
        {
            _account.text = " ";
            _surname.text = " ";
            _name.text = " ";
            _patronymic.text = " ";
        }
        else
        {
            SelectPerson(_curPers);
        }
        SetEditFieldReadOnly(true);
    }

    // ******************************************************************************************

    // Изменение параметров существующего аккаунта
    private void OnChangeClick()
    {
        if (_curPers == "") return;

        AccControlBtnHide();
        SetEditFieldReadOnly(false);
        _account.readOnly = true;       // название аккаунта не меняем при редактировании данных!

        _btnOk.onClick.AddListener(OnChangeOk);
        _btnCancel.onClick.AddListener(OnChangeCancel);
    }

    private void OnChangeOk()
    {
       // записать данные из полей в словарь
        if(_dic.ContainsKey(_curPers) )
        {
            MainMenu.Person curPers = _dic[_curPers];
            curPers.Surname = _surname.text;
            curPers.Name = _name.text;
            curPers.Patronymic = _patronymic.text;
        }
        // обратиться к MainMenu для коррекции файла с данными
        _mainMenu.CorrectAccount(_curPers);

        // вернуться в режим меню
        AccControlBtnRestore();
        SetEditFieldReadOnly(true);
        SelectPerson(_curPers);
    }

    private void OnChangeCancel()
    {
        AccControlBtnRestore();
        SetEditFieldReadOnly(true);
        SelectPerson(_curPers);
    }

    // ******************************************************************************************

    // Очистка истории прохождений уроков
    private void OnCleanClick()
    {
        if (_dic.ContainsKey(_curPers))     // на всякий случай
        {
            // показать окно с предупреждением, Да-Нет
            ShowAlarmMsg("Очистить историю прохождения тем?", AlarmMode.YesNo);
            _alarmBtnYes.onClick.AddListener(OnCleanOk);
        }
    }

    private void OnCleanOk()
    {
        _mainMenu.CleanAccountDate(_curPers);
        HideAlarmPanel();
    }

    // ******************************************************************************************

    // Удаление аккаунта
    private void OnDeleteClick()
    {
        // показать окно с предупреждением, Да-Нет
        if (_dic.ContainsKey(_curPers))     // на всякий случай
        {
            // показать окно с предупреждением, Да-Нет
            ShowAlarmMsg("Удалить аккаунт " + _curPers + "?", AlarmMode.YesNo);
            _alarmBtnYes.onClick.AddListener(OnDeleteOk);
        }
    }

    private void OnDeleteOk()
    {
        _mainMenu.DeleteAccount(_curPers);
        CreateAndSetMenuBtn();
        HideAlarmPanel();
        _txtBlack.text = " ";
        _txtWhite.text = " ";
        _account.text = " ";
        _surname.text = " ";
        _name.text = " ";
        _patronymic.text = " ";
    }

    // ******************************************************************************************

    private void AccControlBtnHide()
    {
        _btnAdd.gameObject.SetActive(false);
        _btnChange.gameObject.SetActive(false);
        _btnDelete.gameObject.SetActive(false);
        _btnClean.gameObject.SetActive(false);

        _menuBlock.SetActive(true);
        _btnOk.gameObject.SetActive(true);
        _btnCancel.gameObject.SetActive(true);
    }

    private void AccControlBtnRestore()
    {
        _btnAdd.gameObject.SetActive(true);
        _btnChange.gameObject.SetActive(true);
        _btnDelete.gameObject.SetActive(true);
        _btnClean.gameObject.SetActive(true);

        _btnOk.onClick.RemoveAllListeners();
        _btnCancel.onClick.RemoveAllListeners();
        _menuBlock.SetActive(false);
        _btnOk.gameObject.SetActive(false);
        _btnCancel.gameObject.SetActive(false);
    }

    private void SetCurPersonDataToViewState()
    {
        if (_curPers != "") // на всякий случай
        {
            SelectPerson(_curPers);
        }
        SetEditFieldReadOnly(true);
    }

    private void SetEditFieldReadOnly(bool isReadOnly)
    {
        _account.readOnly = isReadOnly;
        _surname.readOnly = isReadOnly;
        _name.readOnly = isReadOnly;
        _patronymic.readOnly = isReadOnly;
   }

    // проверка строки на латинские буквы, цифры и _
    private bool IsCorrectString(string str)
    {
        foreach (char ch in str)
        {
            int num = (int)ch;
            if( !( num >= 97 && num <= 122  ||  num >= 65 && num <= 90  ||  num >= 48 && num <= 57  ||  ch =='_' ) )
            {
                return false;
            }

        }
        return true;
    }

    // привязка к переменным спрятанных объектов - панели, кнопок и текстового поля
    private void FindAlarmObjects()
    {
        GameObject canv = GameObject.Find("MenuCanvas");
        for( int i = 0; i < canv.transform.childCount; ++i)
        {
            Transform tr = canv.transform.GetChild(i);
            if(tr.gameObject.name == "AlarmPanel")
            {
                _alarm = tr.gameObject;
            }
        }
        if(_alarm!= null)
        {
            _alarm.SetActive(true);
            _alarmText = GameObject.Find("AlarmPanel/AlarmText").GetComponent<Text>();
            _alarmBtnOk = GameObject.Find("AlarmPanel/AlarmBtnOk").GetComponent<Button>();
            _alarmBtnYes = GameObject.Find("AlarmPanel/AlarmBtnYes").GetComponent<Button>();
            _alarmBtnNo = GameObject.Find("AlarmPanel/AlarmBtnNo").GetComponent<Button>();
            _alarmBtnOk.onClick.AddListener(HideAlarmPanel);
            _alarmBtnNo.onClick.AddListener(HideAlarmPanel);
            _alarm.SetActive(false);
        }
    }

    private void ShowAlarmMsg(string msg, AlarmMode mode)
    {
        _alarm.SetActive(true);
        _alarmText.text = msg;
        if (mode == AlarmMode.Ok)
        {
            _alarmBtnOk.gameObject.SetActive(true);
            _alarmBtnYes.gameObject.SetActive(false);
            _alarmBtnNo.gameObject.SetActive(false);
        }
        else if(mode == AlarmMode.YesNo)
        {
            _alarmBtnOk.gameObject.SetActive(false);
            _alarmBtnYes.gameObject.SetActive(true);
            _alarmBtnNo.gameObject.SetActive(true);
        }
    }

    private void HideAlarmPanel()
    {
        _alarm.SetActive(false);
        _alarmBtnYes.onClick.RemoveAllListeners();

    }

}
