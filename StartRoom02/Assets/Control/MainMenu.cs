using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    // логин ученика, будет использоваться для определения пути к папке с world_xxx.xml
    private string _userLogin = "";
    // имя файла настроек
    private string _settingName = "settings.xml";
    // имя файла с данными обучаемого
    private string _accFile = "userdata.xml";

    private ScriptHerder _scriptHerder;
    private WorldController _worldController;
    private ScrHerder _scrHerder;
    // Общие параметры и методы
    MyGlobals _myGlobals;

    // Элементы меню - view
    private GameObject _accounts;
    private GameObject _lessons;
    private GameObject _tuning;
    // Элементы меню - toggle кнопки
    private GameObject _accountsBtn;
    private GameObject _lessonsBtn;
    private GameObject _tuningBtn;
    // Элементы меню вместе с кнопкой
    private GameObject _accountsFull;
    private GameObject _lessonsFull;
    private GameObject _tuningFull;

    // строка наверху
    private GameObject _blackText;
    private GameObject _whiteText;
    // кнопка скрыть-показать
    private GameObject _btnShowHide;
    private GameObject _btnShowHideText;

    // Учетные записи
    public Dictionary<string, Person> AccountsDic;
    // Файлы уроков
    public Dictionary<string, XDocument> LessonsDic;

    void Awake()
    {
        // получить ссылки на основные скрипты
        GameObject boss = GameObject.Find("Boss");
        //_scriptHerder = boss.GetComponent<ScriptHerder>();
        _scrHerder = boss.GetComponent<ScrHerder>();
        _worldController = boss.GetComponent<WorldController>();
        _myGlobals = boss.GetComponent<MyGlobals>();

        // получить ссылки на элементы меню
        _accounts = GameObject.Find("AccountsView");
        _accountsBtn = GameObject.Find("Accounts/TopMenuBtn");
        _lessons = GameObject.Find("AllMenuLesson");
        _lessonsBtn = GameObject.Find("Lessons/TopMenuBtn");
        _tuning = GameObject.Find("TuningView");
        _tuningBtn = GameObject.Find("Tuning/TopMenuBtn");

        _accountsFull = GameObject.Find("MenuCanvas/Accounts");
        _lessonsFull = GameObject.Find("MenuCanvas/Lessons");
        _tuningFull = GameObject.Find("MenuCanvas/Tuning");

        // получить ссылки на дополнительные элементы
        _blackText = GameObject.Find("MenuCanvas/LessonNameBlack");
        _whiteText = GameObject.Find("MenuCanvas/LessonNameWhite");
        _btnShowHide = GameObject.Find("MenuCanvas/Btn_ShowHide");
        _btnShowHideText = GameObject.Find("MenuCanvas/Btn_ShowHide/Text");
    }

    private void Start()
    {
        _accounts.SetActive(false);
        _lessons.SetActive(false);
        _tuning.SetActive(false);

        SetLessonString("");
        _btnShowHide.GetComponent<Button>().onClick.AddListener(ShowHideAllMenuParts);

        // Считать файл настроек, передать установки в _myGlobals
        LoadSettings();
        // Просканировать папку с пользователями, создать словарь <название аккаунта, Person>, заполнить скролл
        LoadAccounts();
        // Просканировать папку с уроками, скачать их и передать для создания меню
        LoadLessons();
    }


    // как будто был нажат пункт меню
    public void StartLoad()
    {
        // это будет определятся по нажатому пункту меню
        string topicPartStep = "10.10.10";

        // имя файла описания вида world_101010.xml
        string worldName;
        // полный путь к файлу - описанию мира
        string worldFilePath;

        print(" ====== MainMenu -> Кнопка Загрузка -> StartLoad() ====== ");

        // генерируем имя файла мира вида world_101010.xml
        string[] worldNameParts = topicPartStep.Split('.');
        worldName = "world_" + worldNameParts[0] + worldNameParts[1] + worldNameParts[2] + ".xml";

        // Полный путь к файлу XML состояния мира
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath);
        if (_userLogin == "")
        {
            worldFilePath = Path.Combine(dataPath, _myGlobals.myXWorldPath, worldName);
        }
        else
        {
            // TODO: определять, что этот файл существует, и если да, то грузить его
            worldFilePath = Path.Combine(dataPath, _myGlobals.myXWorldPath, _userLogin, worldName);
            // а если нет, то грузить из общей папки
        }

        // загрузить состояние мира (всех контролов), инициализировать их
        _worldController.Load(worldFilePath);

        /*
        // Полный путь к файлу XML сценария
        lessonFilePath = Path.Combine(Directory.GetCurrentDirectory(), XMLDataPath, XMLLessonsPath, XMLLessonFileName);
        */

        // загрузить сценарий и запустить его с требуемого места
        _scrHerder.Load(topicPartStep);

        gameObject.SetActive(false);        // спрятать меню
    }

    // Прячет/показывает все элементы меню
    private void ShowHideAllMenuParts()
    {
        if (_btnShowHideText.GetComponent<Text>().text == "<" )
        {
            _accountsFull.SetActive(false);
            _lessonsFull.SetActive(false);
            _tuningFull.SetActive(false);
            _blackText.SetActive(false);
            _whiteText.SetActive(false);
            _btnShowHideText.GetComponent<Text>().text = ">";
        }
        else if(_btnShowHideText.GetComponent<Text>().text == ">")
        {
            _accountsFull.SetActive(true);
            _lessonsFull.SetActive(true);
            _tuningFull.SetActive(true);
            _blackText.SetActive(true);
            _whiteText.SetActive(true);
            _btnShowHideText.GetComponent<Text>().text = "<";
        }
    }

    // Вызывается, когда была нажата кнопка верхнего меню
    public void TopMenuShow(string menuType)
    {
        print(menuType);
        switch (menuType)
        {
            case "tuning":
                _accounts.SetActive(false);
                _lessons.SetActive(false);
                _accountsBtn.GetComponent<TopMenuToggle>().SetNorm();
                _lessonsBtn.GetComponent<TopMenuToggle>().SetNorm();
                _tuning.SetActive(true);
                _tuning.GetComponent<MenuTuning>().GetValuesFromGlobals();
                break;
            case "account":
                _lessons.SetActive(false);
                TuningSetActive(false);
                _tuningBtn.GetComponent<TopMenuToggle>().SetNorm();
                _lessonsBtn.GetComponent<TopMenuToggle>().SetNorm();
                _accounts.SetActive(true);
                break;
            case "lessons":
                _accounts.SetActive(false);
                TuningSetActive(false);
                _accountsBtn.GetComponent<TopMenuToggle>().SetNorm();
                _tuningBtn.GetComponent<TopMenuToggle>().SetNorm();
                _lessons.SetActive(true);
                _lessons.GetComponent<MenuLessons>().Show();
                break;
        }
    }

    // Вызывается, когда была отжата кнопка верхнего меню
    public void TopMenuHide(string menuType)
    {
        switch (menuType)
        {
            case "tuning":
                TuningSetActive(false);
                break;
            case "account":
                _accounts.SetActive(false);
                break;
            case "lessons":
                _lessons.SetActive(false);
                break;
        }
    }

    // Настройки ************************************************************************************************

    // класс для сохранения-считывания    
    [Serializable]
    public class Settings
    {
        public Settings() { }
        public bool isTips;
        public bool isInstruc;
        public float sndVolume;      
        public bool isJoystick;
    }

    // Считать файл настроек, передать установки в _myGlobals
    public void LoadSettings()
    {
        string settingPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _settingName);
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(Settings));
        Settings set;
        using (XmlReader myXmlRdr = XmlReader.Create(settingPath))
        {
            set = myXmlSrlzr.Deserialize(myXmlRdr) as Settings;
        }
        _myGlobals.isTips = set.isTips;
        _myGlobals.isJoystick = set.isJoystick;
        _myGlobals.isInstruc = set.isInstruc;
        _myGlobals.sndVolume = set.sndVolume;
        print("isTips=" + set.isTips + " isJoystick=" + set.isJoystick + " isInstruc = " + set.isInstruc + " sndVolume = " + set.sndVolume);
    }

    // Взять установки из _myGlobals и сохранить в файл настроек
    public void SaveSettings()
    {
        Settings set = new Settings
        {
            isTips = _myGlobals.isTips,
            isJoystick = _myGlobals.isJoystick,
            isInstruc = _myGlobals.isInstruc,
            sndVolume = _myGlobals.sndVolume
        };
        string settingPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _settingName);
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(Settings));
        XmlWriterSettings myXmlSettings = new XmlWriterSettings();
        myXmlSettings.Encoding = System.Text.Encoding.UTF8;            
        myXmlSettings.Indent = true;
        using (XmlWriter myXmlWrtr = XmlWriter.Create(settingPath, myXmlSettings))
        {
            myXmlSrlzr.Serialize(myXmlWrtr, set);
        }

    }

    // при закрытии меню настроек сохранять в файл текущие настройки
    private void TuningSetActive(bool isActiv)
    {
        _tuning.SetActive(isActiv);
        if(!isActiv)
        {
            SaveSettings();
        }
    }

    // Аккаунты ********************************************************************************************

    // класс для хранения данных аккаунта обучаемого
    [Serializable]
    public class Person
    {
        public Person() { }
        public string AccountName;
        public string Surname;         // фамилия
        public string Name;            // имя
        public string Patronymic;      // отчество
    }

    // Загрузить данные по всем акаунтам в словарь, создать меню
    private void LoadAccounts()
    {
        print("LoadAccounts()");
        string AccPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _myGlobals.myXWorldPath);
        // получить список каталогов в данном
        List<string> dirs = new List<string>(Directory.EnumerateDirectories(AccPath));
        AccountsDic = new Dictionary<string, Person>();
        foreach (var dir in dirs)
        {
            string accName = dir.Substring(dir.LastIndexOf('\\') + 1);
            print(accName);
            string dataFilePath = Path.Combine(dir, _accFile);
            XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(Person));
            Person pers;
            using (XmlReader myXmlRdr = XmlReader.Create(dataFilePath))
            {
                pers = myXmlSrlzr.Deserialize(myXmlRdr) as Person;
            }
            AccountsDic.Add(accName, pers);
            //print(AccountsDic[accName].Surname);
        }
        _accounts.GetComponent<MenuAccount>().Create(ref AccountsDic);
    }

    // Добавление пользователя (вызывается из MenuAccount)
    public void AddAccount(string persName)
    {
        print("AddAccount()");
        Person pers = AccountsDic[persName];

        // создаем папку для нового пользователя
        string AccPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _myGlobals.myXWorldPath);
        string DirPath = Path.Combine(AccPath, pers.AccountName);
        Directory.CreateDirectory(DirPath);

        // сохраняем в эту папку файл с данными аккаунта
        string dataFilePath = Path.Combine(DirPath, _accFile);
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(Person));
        XmlWriterSettings myXmlSettings = new XmlWriterSettings();
        myXmlSettings.Encoding = System.Text.Encoding.UTF8;            
        myXmlSettings.Indent = true;

        using (XmlWriter myXmlWrtr = XmlWriter.Create(dataFilePath, myXmlSettings))
        {
            myXmlSrlzr.Serialize(myXmlWrtr, pers);
        }
    }

    // Изменить данные в аккаунте (вызывается из MenuAccount)
    public void CorrectAccount(string persName)
    {
        print("CorrectAccount()");
        Person pers = AccountsDic[persName];

        string AccPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _myGlobals.myXWorldPath);
        string dataFilePath = Path.Combine(AccPath, pers.AccountName, _accFile);

        // сохраняем в эту папку файл с данными аккаунта
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(Person));
        XmlWriterSettings myXmlSettings = new XmlWriterSettings();
        myXmlSettings.Encoding = System.Text.Encoding.UTF8;
        myXmlSettings.Indent = true;

        using (XmlWriter myXmlWrtr = XmlWriter.Create(dataFilePath, myXmlSettings))
        {
            myXmlSrlzr.Serialize(myXmlWrtr, pers);
        }
    }

    // Удалить файлы с историей прохождения уроков (вызывается из MenuAccount)
    public void CleanAccountDate(string persName)
    {
        print("CleanAccountDate()");
        Person pers = AccountsDic[persName];
        string AccPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _myGlobals.myXWorldPath);
        string dataDirPath = Path.Combine(AccPath, pers.AccountName );

        var xmlFiles = Directory.EnumerateFiles(dataDirPath, "*.xml");
        foreach (string currentFile in xmlFiles)
        {
            string fileName = currentFile.Substring(dataDirPath.Length + 1);
            if(fileName != _accFile)
            {
                //print(currentFile);
                File.Delete(currentFile);
            }
        }
    }

    // Удалить файлы с историей прохождения уроков (вызывается из MenuAccount)
    public void DeleteAccount(string persName)
    {
        print("CleanAccountDate()");
        Person pers = AccountsDic[persName];
        string AccPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _myGlobals.myXWorldPath);
        string dataDirPath = Path.Combine(AccPath, pers.AccountName);
        Directory.Delete(dataDirPath, true);    // удалить папку вместе с содержимым
        AccountsDic.Remove(persName);
    }

    // Уроки *****************************************************************************************

    // Раздел документа 
    XElement myPart;
    // Шаг
    XElement myStep;
    // Просканировать папку с уроками, скачать их и передать для создания меню

    private void LoadLessons()
    {
        print("LoadLessons()");
        LessonsDic = new Dictionary<string, XDocument>();

        string LesPath = Path.Combine(Directory.GetCurrentDirectory(), _myGlobals.myDataPath, _myGlobals.myXFilePath );
        var xmlFiles = Directory.EnumerateFiles(LesPath, "*.xml");
        foreach (string currentFile in xmlFiles)
        {
            string fileName = currentFile.Substring(LesPath.Length + 1);
            try
            {
                XDocument myXdocLesson = XDocument.Load(currentFile);
                print("XML Document загружен " + currentFile);
                string btnNum = myXdocLesson.Root.Attribute("num").Value;
                print(btnNum);
                LessonsDic.Add(btnNum, myXdocLesson);
            }
            catch (System.Exception e)
            {
                print("XML Document " + currentFile + " load error: " + e.Message);
            }
        }
        _lessons.GetComponent<MenuLessons>().CreateLessonsMenu(LessonsDic);
    }

    // установить верхнюю строку с названием 
    public void SetLessonString(string lessonName)
    {
        _blackText.GetComponent<Text>().text = lessonName;
        _whiteText.GetComponent<Text>().text = lessonName;
    }

    // Начать прохождени урока
    public void StartLesson( string lesNum, string partNum, string stepNum )
    {
        print("StartLesson  " + lesNum + "." + partNum + "." + stepNum);
        _lessonsBtn.GetComponent<TopMenuToggle>().SetNorm();
        _lessons.SetActive(false);

        XDocument curLes = LessonsDic[lesNum];





        //_scriptHerder.StartLesson(curLes, partNum, stepNum);
    }

}
