using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UI_Instructions : MonoBehaviour {

    // Время, за которое панель инструкций возвращается в заданное положение в поле зрения
    [SerializeField]
    float myCenterTime = 0.5f;
    // Угол (рыскание), на который смещена инструкция в поле зрения
    [SerializeField]
    float myYaw0 = -15.0f;
    // Расстояние от камеры, на котором висит инструкция
    [SerializeField]
    float myDistance = 0.8f;

    // Компоненты интерфейса

    // Канвас для размещения всех объектов отображения инструкции
    Transform myCanvasTr;
    // Для вывода имени аккунта
    Text myAccText;
    // Для вывода названия темы
    Text myTopicText;
    // Для вывода названия раздела
    Text myPartText;
    // Для отображения шагов
    Text myStepText;
    // Для вывода инструкций
    Text myInstrText;
    // Группа для навигации по страницам
    Transform myPageNavTr;
    // Для отображения текущей страницы
    Text myPageText;
    // Для перехода на следующую страницу
    Button myNextButt;
    // Для перехода на предыдущую страницу
    Button myPrevButt;
    // Для скрытия панели инструкций
    Button myHideButt;

    // Скорости поворота UI канваса за камерой. Требуются для работы функции Mathf.SmoothDampAngle
    float myVelocityX = 0.0F;
    float myVelocityY = 0.0F;
    float myVelocityZ = 0.0F;

    // Корутина для удержания панели инструкций в поле зрения
    Coroutine myCor;

    // Use this for initialization
    void Start () {

        // Соберем ссылки на компоненты интерфейса

        // Канвас для размещения всех объектов отображения инструкции
        myCanvasTr = transform.Find("Canvas_Instructions");
        // Для вывода имени аккунта
        myAccText = myCanvasTr.Find("Text_AccountName").GetComponent<Text>();
        // Для вывода названия темы
        myTopicText = myCanvasTr.Find("Text_TopicName").GetComponent<Text>();
        // Для вывода названия раздела
        myPartText = myCanvasTr.Find("Text_PartName").GetComponent<Text>();
        // Для отображения шагов
        myStepText = myCanvasTr.Find("Text_Steps").GetComponent<Text>();
        // Для вывода инструкций
        myInstrText = myCanvasTr.Find("Text_Instructions").GetComponent<Text>();
        // Группа для навигации по страницам
        myPageNavTr = myCanvasTr.Find("Group_PageNav");
        // Для отображения текущей страницы
        myPageText = myPageNavTr.Find("Text_Pages").GetComponent<Text>();
        // Для перехода на следующую страницу
        myNextButt = myPageNavTr.Find("Button_Next").GetComponent<Button>();
        // Для перехода на предыдущую страницу
        myPrevButt = myPageNavTr.Find("Button_Prev").GetComponent<Button>();
        // Для скрытия панели инструкций
        myHideButt = myCanvasTr.Find("Button_Hide").GetComponent<Button>();

        // Выключить UI канвас по умолчанию
        myCanvasTr.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
		
	}


    // Держать инструкцию в поле зрения
    IEnumerator MyFuncKeepMessage()
    {
        yield return null; // подождать до следующего кадра

        while (true)
        {

            // Держать инструкцию в поле зрения

            // Запомнить родителя
            Transform myParent = transform.parent;
            // Перевести себя в дочерние объекты камеры
            transform.SetParent(Camera.main.transform);
            // Совместить себя с камерой
            transform.localPosition = Vector3.zero;
            // Текущие углы себя относительно камеры
            Vector3 myEu = transform.localEulerAngles;
            //myEu.y += 15.0f;
            // Новые значения углов
            myEu.x = Mathf.SmoothDampAngle(myEu.x, 0.0f, ref myVelocityX, myCenterTime);
            myEu.y = Mathf.SmoothDampAngle(myEu.y, myYaw0, ref myVelocityY, myCenterTime);
            myEu.z = Mathf.SmoothDampAngle(myEu.z, 0.0f, ref myVelocityZ, myCenterTime);
            transform.localEulerAngles = myEu;
            // Передвинуть себя на myDistance вперед
            transform.Translate(Vector3.forward * myDistance);
            // Откорректировать, чтобы угол крена был 0
            myEu = transform.eulerAngles;
            myEu.z = 0.0f;
            transform.eulerAngles = myEu;
            // Вернуть себя обратно родителю
            transform.SetParent(myParent);

            yield return null; // подождать до следующего кадра
        }
    }


    // ========================= Публичные методы =====================================

    // Активировать объект
    public void Show()
    {
        // Если панель инструкций (проверяем канвас) не активна
        if (!myCanvasTr.gameObject.activeSelf)
        {
            // Включить UI канвас
            myCanvasTr.gameObject.SetActive(true);
            // Запустить корутину отображения панели инструкций
            myCor = StartCoroutine(MyFuncKeepMessage());
        }
    }

    // Деактивировать объект
    public void Hide()
    {
        // Если панель инструкций (проверяем канвас) активна
        if (myCanvasTr.gameObject.activeSelf)
        {
            // Остановить корутину отображения панели инструкций
            StopCoroutine(myCor);
            // Выключить UI канвас
            myCanvasTr.gameObject.SetActive(false);
        }
    }

    // Текст заголовка - имя текущего аккаунта
    public void MyAccText(string myText)
    {
        myAccText.text = myText;
    }

    // Название темы
    public void MyTopicText(string myText)
    {
        myTopicText.text = myText;
    }

    // Название раздела
    public void MyPartText(string myText, int myStepsCount)
    {
        myPartText.text = myText;
        // Вывести число шагов пустыми клетками
        myStepText.text = new string('□', myStepsCount); // одиночные кавычки определят тип char
    }

    // Отобразить текущий шаг
    public void MyStep(int myStepNumber)
    {
        // Специальная строка для отображения шагов символами ■ и □.
        // Просто считаем ее из текстового поля (полагаем, что число шагов уже задано предыдущим методом MyPartText)
        StringBuilder mySteps = new StringBuilder(myStepText.text);
        // Перепишем посимвольно
        for(int i = 0; i < mySteps.Length; i++)
        {
            if(i <= myStepNumber - 1) // темные клетки - до текущего шага включительно
            {
                mySteps[i] = '■'; // одиночные кавычки определят тип char
            }
            else
            {
                mySteps[i] = '□';
            }
        }
        myStepText.text = mySteps.ToString();
    }

    // Инструкция
    public void MyInstr(string[] myText)
    {
        int myPagesCount = myText.Length;
        if (myPagesCount > 1)
        {
            myPageNavTr.gameObject.SetActive(true);
        }
        else
        {
            myPageNavTr.gameObject.SetActive(false);
        }
        myInstrText.text = myText[0];
    }
}

