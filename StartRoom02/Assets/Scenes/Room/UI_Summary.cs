using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Summary : MonoBehaviour {

    // Время, за которое панель резюме возвращается в заданное положение в поле зрения
    [SerializeField]
    float myCenterTime = 0.5f;
    // Угол (рыскание), на который смещена панель резюме в поле зрения
    [SerializeField]
    float myYaw0 = 0.0f;
    // Расстояние от камеры, на котором висит резюме
    [SerializeField]
    float myDistance = 0.8f;

    // Компоненты интерфейса

    // Канвас для размещения всех объектов отображения резюме
    Transform myCanvasTr;
    // Для вывода имени аккунта
    Text myAccText;
    // Для вывода названия темы
    Text myTopicText;
    // Для вывода названия раздела
    Text myPartText;
    // Для вывода текста резюме
    Text mySummaryText;
    // Кнопка "Продолжить"
    Button myContinueButt;

    // Скорости поворота UI канваса за камерой. Требуются для работы функции Mathf.SmoothDampAngle
    float myVelocityX = 0.0F;
    float myVelocityY = 0.0F;
    float myVelocityZ = 0.0F;

    // Корутина для удержания панели инструкций в поле зрения
    Coroutine myCor;


    // Use this for initialization
    void Start()
    {

        // Соберем ссылки на компоненты интерфейса

        // Канвас для размещения всех объектов отображения инструкции
        myCanvasTr = transform.Find("Canvas_Summary");
        // Для вывода имени аккунта
        myAccText = myCanvasTr.Find("Text_AccountName").GetComponent<Text>();
        // Для вывода названия темы
        myTopicText = myCanvasTr.Find("Text_TopicName").GetComponent<Text>();
        // Для вывода названия раздела
        myPartText = myCanvasTr.Find("Text_PartName").GetComponent<Text>();
        // Для вывода резюме
        mySummaryText = myCanvasTr.Find("Text_Summary").GetComponent<Text>();
        // Кнопка "Продолжение"
        myContinueButt = myCanvasTr.Find("Text_Continue").GetComponent<Button>();

        // Выключить UI канвас по умолчанию
        myCanvasTr.gameObject.SetActive(false);

    }


    // Держать резюме в поле зрения
    IEnumerator MyFuncKeepMessage()
    {
        yield return null; // подождать до следующего кадра

        while (true)
        {

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
        // Если панель резюме (проверяем канвас) не активна
        if (!myCanvasTr.gameObject.activeSelf)
        {
            // Включить UI канвас
            myCanvasTr.gameObject.SetActive(true);
            // Запустить корутину отображения панели резюме
            myCor = StartCoroutine(MyFuncKeepMessage());
        }
    }

    // Деактивировать объект
    public void Hide()
    {
        // Если панель резюме (проверяем канвас) активна
        if (myCanvasTr.gameObject.activeSelf)
        {
            // Остановить корутину отображения панели резюме
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
    public void MyPartText(string myText)
    {
        myPartText.text = myText;
    }

    // Текст резюме
    public void MySummaryText(string myText)
    {
        mySummaryText.text = myText;
    }


}
