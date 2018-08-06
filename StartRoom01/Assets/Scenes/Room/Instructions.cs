using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour {

    // Время, за которое панель инструкций возвращается в центр поля зрения
    [SerializeField]
    float myCenterTime = 0.5f;

    // Компоненты интерфейса

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
    Transform myPageNav;
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

    // Флаг отображения панели инструкций
    bool myInstrIsActive = true;

    // Настройки и начальные данные проекта
    // sIniSet myIni;

    int myDepth = 0;

    // Use this for initialization
    void Start () {

        // Соберем ссылки на компоненты интерфейса
        // Для вывода имени аккунта
        myAccText = transform.Find("Text_AccountName").GetComponent<Text>();
        // Для вывода названия темы
        myTopicText = transform.Find("Text_TopicName").GetComponent<Text>();
        // Для вывода названия раздела
        myPartText = transform.Find("Text_PartName").GetComponent<Text>();
        // Для отображения шагов
        myStepText = transform.Find("Text_Steps").GetComponent<Text>();
        // Для вывода инструкций
        myInstrText = transform.Find("Text_Instructions").GetComponent<Text>();
        // Группа для навигации по страницам
        myPageNav = transform.Find("Group_PageNav");
        print("myPageNav = " + myPageNav);
        // Для отображения текущей страницы
        myPageText = myPageNav.Find("Text_Pages").GetComponent<Text>();
        // Для перехода на следующую страницу
        myNextButt = myPageNav.Find("Button_Next").GetComponent<Button>();
        // Для перехода на предыдущую страницу
        myPrevButt = myPageNav.Find("Button_Prev").GetComponent<Button>();
        // Для скрытия панели инструкций
        myHideButt = transform.Find("Button_Hide").GetComponent<Button>();

        StartCoroutine(MyFuncKeepMessage());
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
            // Текущее время
            float myTime = Time.time;

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
            myEu.y = Mathf.SmoothDampAngle(myEu.y, -15.0f, ref myVelocityY, myCenterTime);
            myEu.z = Mathf.SmoothDampAngle(myEu.z, 0.0f, ref myVelocityZ, myCenterTime);
            transform.localEulerAngles = myEu;
            // Передвинуть себя на 0.8 метра вперед
            transform.Translate(Vector3.forward * 0.8f);
            // Откорректировать, чтобы угол крена был 0
            myEu = transform.eulerAngles;
            myEu.z = 0.0f;
            transform.eulerAngles = myEu;
            // Вернуть себя обратно родителю
            transform.SetParent(myParent);

            yield return null; // подождать до следующего кадра
        }
    }
}
