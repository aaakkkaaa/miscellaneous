using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;


public class ScrHerder : MonoBehaviour
{

    //================= UI курсанта ================

    // Родительский объект для элементов UI курсанта
    Transform myWorldUI;
    
    // Для работы с инструкциями
    Transform myInstrTr;
    UI_Instructions myInstrUI;

    // Для работы с резюме
    Transform mySummaryTr;
    UI_Summary mySummaryUI;

    // Общие параметры и методы
    MyGlobals myGlobals;

    // Подсветка
    Transform myLightBallTr;

    //================= Для работы с XML ================

    // Имя папки для файлов данных проекта. Перенести в общеизвестное место и оформить, как паблик?
    [SerializeField]
    string myDataPath = "XML_Data";

    // Имя подпапки для файлов уроков. Перенести в общеизвестное место и оформить, как паблик?
    [SerializeField]
    string myXFilePath = "Lessons";

    // Имя файла XML
    [SerializeField]
    string myXFileName = "LearnTheSim_AKA.xml";

    // Полный путь к файлу XML
    string myXFilePathName;

    // Документ XML
    XDocument myXdocLesson;
    // Раздел документа 
    XElement myPart;
    // Шаг
    XElement myStep;
    // Действие
    XElement myAct;

    //====================================================

    // Use this for initialization
    void Start()
    {

        //================= UI курсанта ================

        // Родительский объект для элементов UI курсанта
        myWorldUI = GameObject.Find("UI_World").transform;
        // Для работы с инструкциями
        myInstrTr = GameObject.Find("Instructions").transform;
        myInstrUI = myInstrTr.GetComponent<UI_Instructions>();
        // Для работы с резюме
        mySummaryTr = GameObject.Find("Summary").transform;
        mySummaryUI = mySummaryTr.GetComponent<UI_Summary>();
        // Подсветка
        myLightBallTr = GameObject.Find("HighLightBall").transform;
        myLightBallTr.gameObject.SetActive(false);

        //===============================================


        // Общие параметры и методы
        myGlobals = GameObject.Find("Boss").GetComponent<MyGlobals>();

        // Полный путь к файлу XML
        myXFilePathName = Path.Combine(Directory.GetCurrentDirectory(), myDataPath, myXFilePath, myXFileName);

    }

    // ==========================================================================================================

    // Update is called once per frame
    void Update()
    {

        // Отладка. Вызовы методов по разным клавишам

        // Загрузить документ XML с уроком
        if (Input.GetKeyDown("l"))
        {
            myXdocLesson = MyLoadXML(myXFilePathName);
            if (myXdocLesson == null)
            {
                myGlobals.GlShowMessage("Ошибка загрузки учебной темы"); // Заменить на вывод в экранный UI
            }
        }

        // Встать на определенный шаг
        else if (Input.GetKeyDown("s"))
        {
            MyGoToStep("10.10.10"); // тема.раздел.шаг
        }

        // Включить инструкцию
        else if (Input.GetKeyDown("i"))
        {
            myInstrUI.MyAccText("Сидоров");
            myInstrUI.MyTopicText("Освоение тренажера");
            myInstrUI.MyPartText("Контроллеры", 10);
            myInstrUI.MyStep(1);
            string[] myIns = { "Текст инструкции.\nТекст инструкции.\nТекст инструкции. Текст инструкции. Текст инструкции." };
            myInstrUI.MyInstr(myIns);

            myInstrUI.Show();
        }
        // Выключить инструкцию
        else if (Input.GetKeyDown("k"))
        {
            myInstrUI.Hide();
        }
        // Включить резюме раздела
        else if (Input.GetKeyDown("r"))
        {
            mySummaryUI.Show();
        }
        // Выключить резюме раздела
        else if (Input.GetKeyDown("t"))
        {
            mySummaryUI.Hide();
        }
        // Вызвать тестовое сообщение 1
        else if (Input.GetKeyDown("1"))
        {
            myGlobals.GlShowMessage("Нажато один. Привет, мир!"); // Публикуем событие - вывод сообщения
        }
        // Вызвать тестовое сообщение 2
        else if (Input.GetKeyDown("2"))
        {
            myGlobals.GlShowMessage("Нажато два. Пока, мир!", 1); // Публикуем событие - вывод сообщения
        }
    }

    // ==========================================================================================================

    // Загрузить XML документ
    XDocument MyLoadXML(string myFile)
    {
        try
        {
            XDocument myXDoc = XDocument.Load(myFile);
            print("XML Document загружен");
            // Пропишем название темы, раздела и число шагов в панели инструкции
            myInstrUI.MyTopicText(myXDoc.Root.Attribute("name").Value);
            return myXDoc;
        }
        catch (System.Exception e)
        {
            print("XML Document " + myFile + " load error: " + e.Message);
            return null;
        }
    }

    // ==========================================================================================================

    // Установить урок (тему) на указанный шаг
    void MyGoToStep(string myStepID)
    {
        foreach (XElement myEl in myXdocLesson.Root.Elements("part"))
        {
            print(myEl);
        }
        // Разобрать myStepID на номера темы.раздела.шага
        string[] myIDs = myStepID.Split(new char[] { '.' });
        print("myStep = " + myIDs[0] + " " + myIDs[1] + " " + myIDs[2]);

        // Сформировать имя и путь к файлу снимка
        // ....

        // Загрузить XML файл снимка, десериализовать его в Control и применить к объектам
        // ....

        // Найти в XML документе нужный шаг

        // Сначала найдем раздел (по атрибуту "num")
        IEnumerable<XElement> myParts = from myEl in myXdocLesson.Root.Elements("part") where (string)myEl.Attribute("num") == myIDs[1] select myEl;
        myPart = myParts.First(); // берем только первый (он же должен быть единственный)
        //print("part = " + myPart);

        // Потом найдем шаг. Перебираем с начала шагов данного раздела, так как нам нужно узнать еще порядковый номер указанного шага
        IEnumerable<XElement> mySteps = myPart.Elements("step");
        int myStepNumber = 0; // номер шага
        foreach (XElement myEl in mySteps)
        {
            myStepNumber++;
            if ((string)myEl.Attribute("num") == myIDs[2]) // шаг тоже берем только первый (он же должен быть единственный)
            {
                myStep = myEl;
                break;
            }
        }
        print("step = " + myStep + " number = " + myStepNumber);

        // Пропишем название раздела и число шагов в панели инструкции (название темы прописали сразу при открытии документа)
        myInstrUI.MyPartText(myPart.Attribute("name").Value, mySteps.Count());
        myInstrUI.MyStep(myStepNumber);


        // Возьмем первый action шага
        myAct = myStep.Element("action");
        print("myAct = " + myAct);

        // Начнем реализацию action
        MyActionPrepare(myAct);

    }


    // ==========================================================================================================

    // Первая часть шага - Подготовка 
    void MyActionPrepare(XElement myAct)
    {
        // Выполнить команды для стандартных объектов
        MyCommands(myAct);


        // Выполнить произвольные .


        // Сформировать условия + и условия -


        // Запустить таймер ожидания


    }

    // Выполнить команды для стандартных объектов
    void MyCommands(XElement myEl)
    {
        // Интструкция. Подготовить и включить.
        IEnumerable<XElement> myInstrPages = myEl.Elements("instr"); // все элементы <instr> в данном шаге
        int myPagesCount = myInstrPages.Count(); // сколько в инструкции страниц
        if (myPagesCount > 0)
        {
            string[] myIns = new string[myPagesCount];
            int i = 0;
            foreach (XElement myInstrPage in myInstrPages)
            {
                myIns[i] = myInstrPage.Value;
                i++;
            }
            myInstrUI.MyInstr(myIns);
            myInstrUI.Show();
        }

        // Подсветка
        XElement myLight = myEl.Element("light");
        string myObjPath = myLight.Value;
        Transform myObjTr = GameObject.Find(myObjPath).transform; // ВРЕМЕННО! Заменить на обращение к World
        print("Объект для подсветки: " + myObjPath + ", его трансформ: " + myObjTr);
        // Перевести шарик подсветки в детей целевого объекта, совместить с ним и включить
        myLightBallTr.parent = myObjTr;
        myLightBallTr.localPosition = Vector3.zero;
        myLightBallTr.gameObject.SetActive(true);

        // Куда смотреть

        // Сообщение

    }



    // ==========================================================================================================

    // Обработчик событий объектов, имеющих Control
    void MyControlEvent(string myStepID)
    {

    }


    // ==========================================================================================================

    // Обработчик событий таймера окончания ожидания
    void MyTimerEvent(string myStepID)
    {

    }
}