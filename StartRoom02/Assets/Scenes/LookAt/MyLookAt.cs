using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLookAt : MonoBehaviour {

    // Сумка
    Transform myBagTr;
    // Крышка
    Transform myCoverTr;

    // Переменные сделаны Serialized только для отображения значений в инспекторе

    // Локальное положение "руки" относительно точки крепления крышки сумки
    [SerializeField]
    Vector3 myLocalPos;

    // Угол между осью X сумки и направлением на "руку" из точки крепления крышки сумки
    [SerializeField]
    float myAngle;

    // Use this for initialization
    void Start () {

        // Найти ссылки на трансформы чемодана и крышки
        myBagTr = GameObject.Find("Cargo_CTB_Full_Size").transform;
        myCoverTr = myBagTr.Find("Cargo_CTB_Full_Size.001").transform;
        print("myBagTr = " + myBagTr + " myCoverTr = " + myCoverTr);

    }
	
	// Update is called once per frame
	void Update () {

        // Вычислить локальное положение руки относительно точки крепления крышки сумки
        myLocalPos = myBagTr.InverseTransformPoint(transform.position) - myCoverTr.localPosition;
        // Сделать поправку, чтобы оказаться в верикальной плоскости XY сумки
        myLocalPos.z = 0.0f;
        // Получить угол
        myAngle =Vector3.SignedAngle(myBagTr.right, myLocalPos, myBagTr.forward);

        // Позиционировать крышку сумки
        Vector3 myOri = myCoverTr.localEulerAngles;
        myOri.z = Mathf.Clamp(myAngle, 0, 135); // ограничение вращения крышки
        myCoverTr.localEulerAngles = myOri;

    }
}
