using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Globalization;
using UnityEngine;
using BooleanLogicParser;

public class ScrHerder : MonoBehaviour
{
    // Источник информации об объектах и т.д.
    private WorldController _worldController;
    // Делегат - для получения событий от Control
    public delegate void MyEvent(string NativePath, Transform mySenderTransf);


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

    // Таймер ожидания
    Coroutine myTimer;
    // Дополнительная блокировка таймера
    bool myTimersStopped = true;

    //================= Для работы с XML ================

    // Имя папки для файлов данных проекта. Перенести в общеизвестное место и оформить, как паблик?
    // Ф: Перенес в MyGlobals

/*
    // Имя файла XML
    [SerializeField]
    string myXFileName = "LearnTheSim_AKA.xml";
*/

    // Связь пункта меню и имени файла
    Dictionary<string, string> xmlFiles = new Dictionary<string, string>
    {
        ["10"] = "10_LearnTheSim.xml",
        ["20"] = "20_Progress.xml"
    };

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
        // Источник информации об объектах и т.д.
        _worldController = GetComponent<WorldController>();
        // Подписка на события из Control
        Control.MyStateChanged += MyControlEvent;

    }

    // ==========================================================================================================

    // Update is called once per frame
    void Update()
    {

        // Отладка. Вызовы методов по разным клавишам
        /*
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
        else if (Input.GetKeyDown("g"))
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
        // Проверка Препарсера
        else if (Input.GetKeyDown("3"))
        {
            MyPreParser("![ViveControllerLeft/ButtonGrip.downState=down] or [ViveControllerRight/ButtonGrip.downState=down]");
        }
        */

    }

    // ==========================================================================================================

    // Загрузить XML документ по строке вида 10.10.10 и запустить сценарий
    public void Load( string curStep )
    {
        print("ScrHerder.Load " + curStep);
        string[] stepParts = curStep.Split('.');
        string fileName = xmlFiles[ stepParts[0] ];
        // Полный путь к файлу XML
        myXFilePathName = Path.Combine(Directory.GetCurrentDirectory(), myGlobals.myDataPath, myGlobals.myXFilePath, fileName);

        myXdocLesson = MyLoadXML(myXFilePathName);
        if (myXdocLesson == null)
        {
            myGlobals.GlShowMessage("Ошибка загрузки учебной темы"); // Заменить на вывод в экранный UI
        }
        else
        {
            MyGoToStep(curStep);
        }
    }

    XDocument MyLoadXML(string myFile)
    {
        try
        {
            XDocument myXDoc = XDocument.Load(myFile);
            print("XML Document загружен " + myFile);
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
    public void MyGoToStep(string myStepID)
    {
        foreach (XElement myEl in myXdocLesson.Root.Elements("part"))
        {
            print(myEl);
        }
        // Разобрать myStepID на номера темы.раздела.шага
        string[] myIDs = myStepID.Split(new char[] { '.' });
        print("myStep = " + myIDs[0] + " " + myIDs[1] + " " + myIDs[2]);

        // Найти в XML документе нужный шаг:
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

        // Возможно нажо получить все <action> в виде массива? Чтобы было проше переходить к следующему
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
        MyStdCommands(myAct);           // <action>

        // Выполнить команды для произвольных объектов
        myCustomCommands(myAct);        // <action>

        // Сформировать условия + и условия -
        //...

        // Запустить таймер ожидания
        XElement myWait = myAct.Element("wait");
        if (myWait != null)
        {
            int myWaitTime = 15;
            XAttribute myTime = myWait.Attribute("time");
            if (myTime != null)
            {
                int.TryParse(myTime.Value, out myWaitTime);
            }
            print("myWaitTime = " + myWaitTime);
            myTimersStopped = false; // дополнительная блокировка таймера
            myTimer = StartCoroutine(MyTimer(myWaitTime));
        }

    }

    // Выполнить команды для стандартных объектов
    // myEl это блок <action>....</action> или <wait>...</wait> или <plus>...</plus> или <minus>...</minus>
    void MyStdCommands(XElement myEl)
    {
        print("MyStdCommands " + myEl);
        // Интструкция. Подготовить и включить.
        IEnumerable<XElement> myInstrPages = myEl.Elements("instr"); // все элементы <instr> в данном узле
        int myPagesCount = myInstrPages.Count(); // сколько в инструкции страниц
        // Делаем, если инструкции есть
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
        // Делаем, если подсветка есть
        if(myLight != null)
        {
            string myObjPath = myLight.Value;
            Transform myObjTr = GameObject.Find(myObjPath).transform; // ВРЕМЕННО! Заменить на обращение к World
            print("Объект для подсветки: " + myObjPath + ", его трансформ: " + myObjTr);
            // Перевести шарик подсветки в детей целевого объекта, совместить с ним и включить
            myLightBallTr.parent = myObjTr;
            myLightBallTr.localPosition = Vector3.zero;
            myLightBallTr.gameObject.SetActive(true);
        }

        // Куда смотреть
        XElement myToSee = myEl.Element("tosee");
        if (myToSee != null)
        {
            string myObjPath = myToSee.Value;
            Transform myObjTr = GameObject.Find(myObjPath).transform; // ВРЕМЕННО! Заменить на обращение к World
            print("Объект 'Куда смотреть': " + myObjPath + ", его трансформ: " + myObjTr);
            // Установить глобальное свойство - трансформ, на который желательно смотреть курсанту.
            myGlobals.GlWhatToSeeTr = myObjTr;
        }

        // Сообщение
        XElement myMessage = myEl.Element("message");
        if (myMessage != null)
        {
            myGlobals.GlShowMessage(myMessage.Value);
        }

    }

    // Выполнить команды для произвольных объектов  
    // myEl это блок <action>....</action> или <wait>...</wait> или <plus>...</plus> или <minus>...</minus>
    void myCustomCommands(XElement myEl)
    {
        // Обрабатывем элемент commands в данном узле
        XElement myCommands = myEl.Element("commands");     //<commands>
        // Все элементы object в данном узле - это и есть команды
        IEnumerable <XElement> myObjects = myCommands.Elements("object");
        // Делаем, если команды есть
        if (myObjects.Count() > 0)
        {
            foreach (XElement el in myObjects)
            {
                print("el = " + el);
                // Объект <object>
                //Transform myObjTr = GameObject.Find(el.Attribute("nativePath").Value).transform; // ВРЕМЕННО! Заменить на обращение к WorldController
                Transform myObjTr = _worldController.SourceControls[el.Attribute("nativePath").Value].transform;

                // Состояние активности
                XElement myActive = el.Element("active");
                if(myActive != null)
                {
                    string myActiveState = myActive.Attribute("state").Value;
                    bool myIsActive;
                    if (bool.TryParse(myActiveState, out myIsActive))
                    {
                        myObjTr.gameObject.SetActive(myIsActive);
                    }
                    print("myActiveState = " + myActiveState + " myState = " + myIsActive);
                }

                // Новый родитель
                XElement myParent = el.Element("parent");
                if (myParent != null)
                {
                    XAttribute myParentAttr = myParent.Attribute("par");
                    if (myParentAttr != null)
                    {
                        string myParentPath = myParentAttr.Value;
                        Transform myParTr = GameObject.Find(myParentPath).transform; // НЕ ВРЕМЕННО! Так и надо
                        print("myParentPath = " + myParentPath);

                        myObjTr.parent = myParTr;
                    }
                    else
                    {
                        print("myParentAttribute == null");
                    }
                }
                else
                {
                    print("myParentNode == null");
                }

                // позиция, углы, масштаб
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";
                float myX, myY, myZ;
                XElement myPos = el.Element("pos");
                if (myPos != null)
                {
                    myX = Single.Parse(myPos.Attribute("x").Value, NumberStyles.Any, ci);
                    myY = Single.Parse(myPos.Attribute("y").Value, NumberStyles.Any, ci);
                    myZ = Single.Parse(myPos.Attribute("z").Value, NumberStyles.Any, ci);
                    //float.TryParse(myPos.Attribute("x").Value, out myX );
                    //myX = Convert.ToSingle( myPos.Attribute("x").Value );
                    myObjTr.localPosition = new Vector3(myX, myY, myZ);
                    print("myObjTr.localPosition = " + myObjTr.localPosition);
                }
                XElement myRot = el.Element("rot");
                if (myRot != null)
                {
                    myX = Single.Parse(myRot.Attribute("x").Value, NumberStyles.Any, ci);
                    myY = Single.Parse(myRot.Attribute("y").Value, NumberStyles.Any, ci);
                    myZ = Single.Parse(myRot.Attribute("z").Value, NumberStyles.Any, ci);
                    myObjTr.localEulerAngles = new Vector3(myX, myY, myZ);
                }
                XElement myScale = el.Element("scale");
                if (myScale != null)
                {
                    myX = Single.Parse(myScale.Attribute("x").Value, NumberStyles.Any, ci);
                    myY = Single.Parse(myScale.Attribute("y").Value, NumberStyles.Any, ci);
                    myZ = Single.Parse(myScale.Attribute("z").Value, NumberStyles.Any, ci);
                    myObjTr.localScale = new Vector3(myX, myY, myZ);
                }

                // состояния freeState, openState, downState
                XElement myState = el.Element("state");
                if(myState != null)
                {
                    Control control = myObjTr.gameObject.GetComponent<Control>();
                    XAttribute myFreeState = myState.Attribute("freeState");
                    if (myFreeState != null)
                    {
                        control.SetState("freeState", myFreeState.Value);
                    }
                    XAttribute myOpenState = myState.Attribute("openState");
                    if (myOpenState != null)
                    {
                        control.SetState("openState", myOpenState.Value);
                    }
                    XAttribute myDownState = myState.Attribute("downState");
                    if (myDownState != null)
                    {
                        control.SetState("downState", myDownState.Value);
                    }
                    XAttribute myParam = myState.Attribute("param");
                    if (myParam != null)
                    {
                        string strParam = myParam.Value;
                        float param = Single.Parse(myParam.Value, NumberStyles.Any, ci);
                        control.SetState("param",  param);
                    }
                }
            }
        }
    }

    // Таймер ожидания действий курсанта
    IEnumerator MyTimer(int myWaitTime)
    {
        yield return new WaitForSeconds(myWaitTime);
        print("myTimer Wait " + myWaitTime);
        MyTimerEvent();
    }



    // ==========================================================================================================

    // Обработчик событий объектов, имеющих Control
    void MyControlEvent(string NativePath, Transform mySenderTransf)
    {
        print("Обработчик: OnStateChanged" + ", Полное имя объекта в иерархии сцены: " + NativePath + ", Публикатор: " + mySenderTransf);
        //print(mySenderTransf.position.ToString("F4"));

        IEnumerable<XElement> allPluses = myAct.Elements("plus");
        // проверяю условие <plus> для каждого (узлов <plus> может быть несколько)
        // пример узла <plus condition="[ViveControllerRight/ButtonMenu.downState=down]">
        bool isPlus = false;
        foreach (XElement pl in allPluses)
        {
            string condish = pl.Attribute("condition").Value;
            string expression = MyPreParser(condish);
            print("plus expression = " + expression);
            bool result = ParseSingleToken(expression);
            if(result)
            {
                isPlus = true;
                print("Условие выполнено, будет переход к следующему шагу");
                MyStdCommands(pl);
                MyActionEnd();

                // <next act="0"/> номер следующего action, если 0 - то выход на конец шага 
                XElement myNext = pl.Element("next");
                if (myNext != null)
                {
                    string actNum = myNext.Attribute("act").Value;
                    if(actNum != "0")
                    {
                        IEnumerable<XElement> myActs = from myEl in myStep.Elements("action") where (string)myEl.Attribute("num") == actNum select myEl;
                        myAct = myActs.First();
                        MyActionPrepare(myAct);
                        return;
                    }
                    else
                    {
                        //TODO:  переход к началу следующего шага
                    }
                }
                else
                {
                    IEnumerable<XElement> myNextActs = myAct.ElementsAfterSelf("action");
                    // TODO: если последний action в шаге, а может и последний step 

                }
                

                return;     // на всякий случай
            }
        }
        // Если никакой из плюсов не выполнился, то же самое делаю с минусами
        if (!isPlus)
        {
            IEnumerable<XElement> allMinuses = myAct.Elements("minus");
            foreach (XElement min in allMinuses)
            {
                string condish = min.Attribute("condition").Value;
                string expression = MyPreParser(condish);
                print("minus expression = " + expression);
                bool result = ParseSingleToken(expression);
                if (result)
                {
                    MyStdCommands(min);
                    break;
                    // Достаточно первого обработанного  минуса
                }
            }
        }

    }

    private bool ParseSingleToken(string expression)
    {
        var tokens = new Tokenizer(expression).Tokenize();
        var parser = new Parser(tokens);
        return parser.Parse();
    }

    // Предварительная подготовка логического условия
    // Пример строки myCondition: "[ViveControllerLeft/ButtonGrip.downState=down] or [ViveControllerRight/ButtonGrip.downState=down]"

    // Здесь будем хранить единичные условия и их "координаты" в общей строке
    struct MyPredicate
    {
        public string PrString;
        public int OpenBr;
        public int CloseBr;
    }

    string MyPreParser(string myCondition)
    {

        if (myCondition == "")
        {
            print("MyPreParser: Ошибка! Для подготовки логического условия передана пустая строка.");
            return "";
        }

        // Массив структур для отдельных условий
        List<MyPredicate> myPredicates = new List<MyPredicate>();

        // Соберем выделенные в [...] единичные условия в массив List
        int myStart = 0; // с какого символа начинать поиск
        for(int i=0; i<100; i++) // на всякий случай число возможных предикатов ограничено
        {
            // Ищем [
            int myOpenBracket = myCondition.IndexOf("[", myStart);
            if (myOpenBracket == -1) // Открывающая скобка не найдена
            {
                break; // Больше нет единичных условий
            }
            // Ищем ]
            int myCloseBracket = myCondition.IndexOf("]", myStart);
            if (myCloseBracket == -1) // Закрывающая скобка не найдена
            {
                print("MyPreParser: Ошибка! При подготовке логического условия обнаружена незакрытая квадратная скобка.");
                return "";
            }

            // Вырежем единичное условие и заполним структуру
            MyPredicate myPredicate = new MyPredicate();
            myPredicate.PrString = myCondition.Substring(myOpenBracket + 1, myCloseBracket - myOpenBracket - 1);
            myPredicate.OpenBr = myOpenBracket;
            myPredicate.CloseBr = myCloseBracket;

            //print("myStart = " + myStart + " myOpenBracket = " + myOpenBracket + " myCloseBracket = " + myCloseBracket + " mySubStr = " + myPredicate.PrString);

            // Добавим структуру в массив
            myPredicates.Add(myPredicate);
            // Новый старт для поиска
            myStart = myCloseBracket + 1;
            if(myStart >= myCondition.Length)
            {
                break; // дошли до конца строки
            }
        }

        // Cтрока для формирования окончательного логического выражения
        StringBuilder myCond = new StringBuilder();


  // Обработаем каждый предикат
        for(int i=0; i < myPredicates.Count; i++)
        {
            print(myPredicates[i]);
            string[] mySubStr = myPredicates[i].PrString.Split(new char[] { '.', '=' });
            string myPath = mySubStr[0];
            string propName = mySubStr[1];
            string propValue = mySubStr[2];

            // Получить Control:
            print("myPath = " + myPath);
            Control ctrl = _worldController.SourceControls[myPath];
            bool result;
            // Получить логическое значение предиката
            if ( ctrl != null)
            {
                result = ctrl.GetState(propName, propValue);
            }
            else
            {
                print("MyPreParser: Не удается получить Control по пути: " + myPath);
                result = false;
            }

            // Собрать строку, заменяя в ней условия [...] на их логические значения
            if (i == 0) // могут быть символы до первой открывающейся скобки
            {
                myCond.Append(myCondition.Substring(0, myPredicates[i].OpenBr));
            }
            myCond.Append(result);
            if(i < myPredicates.Count - 1) // это еще не последний предикат
            {
                myCond.Append(myCondition.Substring(myPredicates[i].CloseBr+1, myPredicates[i+1].OpenBr - myPredicates[i].CloseBr - 1));
            }
            else // а вот это уже последний
            {
                myCond.Append(myCondition.Substring(myPredicates[i].CloseBr + 1, myCondition.Length - myPredicates[i].CloseBr - 1));
            }
        }
        print( "MyPreParser return  " + myCond.ToString() );
        return myCond.ToString();
    }


    // ==========================================================================================================

    // Обработчик таймера окончания ожидания
    void MyTimerEvent()
    {
        if(myTimersStopped)
        {
            return;
        }

    }

    // Конец действия action
    void MyActionEnd()
    {
        //Выключить таймер
        myTimersStopped = true; // дополнительная блокировка таймера
        StopCoroutine(myTimer);

        //Выключить подсветку
        myLightBallTr.gameObject.SetActive(false);

        //Выключить "куда смотреть"
        //...

        // на всякий случай, может это не нужно
        myInstrUI.Hide();
        mySummaryUI.Hide();
    }
}