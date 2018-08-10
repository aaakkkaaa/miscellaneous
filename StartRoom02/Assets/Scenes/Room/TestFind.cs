using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFind : MonoBehaviour {

    [SerializeField]
    string myObjectToFind;


    // Update is called once per frame
    void Update () {

        // Запустить поиск клавишей F11
        if (Input.GetKeyDown(KeyCode.F11))
        {
            MyFind(myObjectToFind);
        }
	}

    // Проходит по всем объектам корня сцены, ищет в их детях.
    // В каждом узле находит первое совпадение.
    void MyFind(string myObjName)
    {
        // Получим список корневых объектов сцены
        List<GameObject> myRootObjects = new List<GameObject>();
        Scene myScene = SceneManager.GetActiveScene();
        myScene.GetRootGameObjects(myRootObjects);

        // Просмотреть все объекты корня сцены
        for (int i = 0; i < myRootObjects.Count; ++i)
        {
            print("Корневой объект: " + myRootObjects[i] + " ====================");
            Transform myObjTr = myFuncGetChild(myRootObjects[i].transform, myObjName);

            // Вывести результат
            if(myObjTr != null)
            {
                print("Найден объект " + myObjTr);
            }
            else
            {
                print("Ничего не найдено" + myObjTr);
            }
        }
    }

    //
    Transform myFuncGetChild(Transform Parent, string ChildName)
    {
        //print("Parent = " + Parent);
        for (int i = 0; i < Parent.childCount; i++)
        {
            Transform myObjTr = Parent.GetChild(i);
            //print("Child[" + i + "] = " + myObjTr);
            if (myObjTr.name == ChildName)
            {
                return myObjTr;
            }
            else
            {
                myFuncGetChild(myObjTr, ChildName);
            }
        }
        return null;
    }

}
