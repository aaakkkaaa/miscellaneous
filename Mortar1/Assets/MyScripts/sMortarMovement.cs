
using System;
using UnityEngine;
//using UnityEngine.UI;
/*
 * Клавиатура
 * up,down - вверх, вниз
 * left,right - влево, вправо
 * shift up,down - вперед, назад
 * shift left,right - курс
 * ctrl up,down - тангаж
 * ctrl left,right - крен
 * 
 * mouse wheel - вперед, назад
 * mouse move + mouse left button - вперед, назад, влево, вправо
 * 
 */

public class sMortarMovement : MonoBehaviour
{
    //Вывод в инспектор для контроля
    [SerializeField]
    float myHor;
    [SerializeField]
    float myVert;
    [SerializeField]
    float myMouseWheel;

    // Коэфициенты скоростей
    [SerializeField]
    //[HideInInspector]
    float myTransSpeedFactor = 0.25f; // горизонтальная скорость
    [SerializeField]
    float myAngleSpeedFactor = 1f;  // угловая скорость

    // Параметры перелета

    // Положение в начале перелета
    Vector3 myStartPos;
    Vector3 myStarttEu;
    // Положение в конце перелета
    Vector3 myEndPos;
    Vector3 myEndEu;
    // Флаг перелета, блокирует управление
    bool myFlight = false;
    // Время начала перелета, сек
    float myStartTime;
    // Продолжительность перелета, сек
    [SerializeField]
    float myFlightTime = 2.0f;

    // Положение в начале сеанса
    Vector3 myHomePos;
    Vector3 myHomeEu;

    int myCount = 0;
    bool myDown = false;

    public Camera myCamera; // основная камера сцены
    float myVert2;
    float myHor2;

    void Start()
    {
        // Положение в начале сеанса
        myHomePos = transform.position;
        myHomeEu = transform.eulerAngles;
    }

    private void Update()
    {
        // Блокировка управления включена, только перелетаем в заданное положение
        if (myFlight)
        {
            float myInterpolant = (Time.time - myStartTime) / myFlightTime;
            transform.localPosition = Vector3.Lerp(myStartPos, myEndPos, myInterpolant);
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(myStarttEu), Quaternion.Euler(myEndEu), myInterpolant);
            // Отключаем блокировку управления
            if (myInterpolant > 1)
            {
                myFlight = false;
            }
        }
        // Блокировка управления отключена
        else
        {
            // Команда на перелет домой
            if (Input.GetKeyDown("h"))
            {
                transform.parent = null; // Выйти в корень иерархии сцены
                myStartTime = Time.time;
                myStartPos = transform.position;
                myStarttEu = transform.eulerAngles;
                myEndPos = myHomePos;
                myEndEu = myHomeEu;

                myFlight = true;
            }
            // Все остальное управление
            else
            {

                // Клавиатура или джойстик
                myHor = Input.GetAxis("Horizontal");
                myVert = Input.GetAxis("Vertical");

                // Текущее положение (угловое)
                Vector3 myEu = transform.eulerAngles;

                // Если нажат Ctrl - Вращение
                if (Input.GetKey("left ctrl") || Input.GetKey("right ctrl"))
                {
                    myEu.x += myVert * myAngleSpeedFactor;
                    myEu.z -= myHor * myAngleSpeedFactor;
                    transform.eulerAngles = myEu;
                }
                else if (Input.GetKey("left alt") || Input.GetKey("right alt"))
                {
//                    myEu.y += myHor * myAngleSpeedFactor;
//                    transform.eulerAngles = myEu;
                }
                // Если нажат Shift - Перемещение вперед/назад
                else if (Input.GetKey("left shift") || Input.GetKey("right shift"))
                {
                    myEu.y += myHor * myAngleSpeedFactor;
                    transform.eulerAngles = myEu;
                    transform.Translate(0.0f, 0.0f, myVert * myTransSpeedFactor);
                }
                // Если не нажат Ctrl или Shift - Перемещение в стороны и вверх/вниз
                else
                {
                    transform.Translate(myHor * myTransSpeedFactor, myVert * myTransSpeedFactor, 0.0f);
                }


                if (Input.GetMouseButtonDown(0))
                {
                    //   myCount++;
                    //   Debug.Log("myCount=" + myCount);
                    myDown = true;
                    Debug.Log("Pressed left click.");
                    Cursor.visible = false;

                    //Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

                    //                    Debug.Log("mousePosition=" + Input.mousePosition);

                }

                if (Input.GetMouseButtonUp(0))
                {
                    myDown = false;
                    Debug.Log("Mouse Up");
                    Cursor.visible = true;

                }


                if (Input.GetMouseButtonDown(1))
                    Debug.Log("Pressed right click.");

                if (Input.GetMouseButtonDown(2))
                    Debug.Log("Pressed middle click.");

                if (myDown == true)
                {
                    Debug.Log("mousePosition=" + Input.mousePosition);
                    myVert2 = Input.GetAxis("Mouse Y");
                    myHor2 = Input.GetAxis("Mouse X");
                    transform.Translate(0.0f, 0.0f, myVert2 * myTransSpeedFactor*2);
                    transform.Translate(myHor2 * myTransSpeedFactor*2, 0.0f, 0.0f);
                    //  myEu.z += myHor * myAngleSpeedFactor;
                    //transform.eulerAngles = myEu;
                }


                // Мышь - колесико. Перемещение вперед/назад
                transform.Translate(0.0f, 0.0f, Input.GetAxis("Mouse ScrollWheel") * 5 * myTransSpeedFactor);
            }
        }
    }
}