using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class TestLINQ : MonoBehaviour {

    // Имя папки для файлов данных проекта. Перенести в общеизвестное место и оформить, как паблик?
    [SerializeField]
    string myDataPath = "XML_Data";

    // Имя подпапки для файлов уроков. Перенести в общеизвестное место и оформить, как паблик?
    [SerializeField]
    string myXFilePath = "Lessons";

    // Имя файла XML
    [SerializeField]
    string myXFileName = "LearnTheSim.xml";

    // Полный путь к файлу XML
    string myXFilePathName;

    // Use this for initialization
    void Start () {

        // Полный путь к папке с уроками
        string myXFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), myDataPath, myXFilePath);
        // Создать папки, если их еще нет
        Directory.CreateDirectory(myXFileFullPath);

        // Полный путь к файлу XML
        myXFilePathName = Path.Combine(myXFileFullPath, myXFileName);

        /*
        XDocument xdoc = XDocument.Load("phones.xml");
        foreach (XElement phoneElement in xdoc.Element("phones").Elements("phone"))
        {
            XAttribute nameAttribute = phoneElement.Attribute("name");
            XElement companyElement = phoneElement.Element("company");
            XElement priceElement = phoneElement.Element("price");

            if (nameAttribute != null && companyElement != null && priceElement != null)
            {
                print("Смартфон: " + nameAttribute.Value);
                print("Компания: " + companyElement.Value);
                print("Цена: " + priceElement.Value);
            }
        }
         */
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("x"))
        {
            MyFuncXMLTemplate();
        }
		
	}

    void MyFuncXMLTemplate()
    {
        XDocument xdoc = new XDocument(new XElement("topic", new XAttribute("title", "Освоение тренажера"),
    new XElement("part",
        new XAttribute("num", "1"),
        new XAttribute("name", "Индивидуальная настройка маски VR"),
        new XElement("step",
            new XAttribute("num", "1"),
            new XAttribute("task", "Понять устройство маски VR")),
        new XElement("step",
            new XAttribute("num", "2"),
            new XAttribute("task", "Настроить маску под курсанта"))),
    new XElement("part",
        new XAttribute("num", "2"),
        new XAttribute("name", "Рабочее пространство"),
        new XElement("step",
            new XAttribute("num", "1"),
            new XAttribute("task", "Первый опыт в витруальном пространстве")),
        new XElement("step",
            new XAttribute("num", "2"),
            new XAttribute("task", "Понять, как отображаются в виртуальном пространстве границы рабочей зоны")),
        new XElement("step",
            new XAttribute("num", "3"),
            new XAttribute("task", "Понять принципы трекинга и проблему затенения")),
        new XElement("step",
            new XAttribute("num", "4"),
            new XAttribute("task", "Понять условность работы в модели МКС"))),
    new XElement("part",
        new XAttribute("num", "3"),
        new XAttribute("name", "Контроллеры"),
        new XElement("step",
            new XAttribute("num", "1"),
            new XAttribute("task", "Понять устойство контроллеров")),
        new XElement("step",
            new XAttribute("num", "2"),
            new XAttribute("task", "Продолжение изучения контроллеров в VR")),
        new XElement("step",
            new XAttribute("num", "3"),
            new XAttribute("task", "Кнопка Меню правого контроллера"),
            new XElement("action",
                new XAttribute("num", "1"),
                new XElement("instr", "На правом контроллере нажмите большим пальцем кнопку \"Меню\"."),
                new XElement("light", "Кнопка Меню на правом контроллере"),
                new XElement("tosee", "Правый контроллер"),
                new XElement("towait",
                    new XAttribute("sender", "Правый контроллер, кнопка Меню"),
                    new XAttribute("event", "Нажатие кнопки"),
                    new XElement("plus",
                        new XElement("message", "Кнопка \"Меню\" нажата")),
                    new XElement("minus",
                        new XElement("message", "Нажмите кнопку \"Меню\".")))),
            new XElement("action",
                new XAttribute("num", "2"),
                new XElement("instr", "Вы нажали кнопку \"Меню\". Чтобы продолжить нажмите кнопку \"Меню\" еще раз."),
                new XElement("tosee", "Правый контроллер"),
                new XElement("towait",
                    new XAttribute("sender", "Правый контроллер, кнопка Меню"),
                    new XAttribute("event", "Нажатие кнопки"),
                    new XElement("minus",
                        new XElement("light", "Кнопка Меню на правом контроллере"),
                        new XElement("message", "Нажмите кнопку \"Меню\"."))))),
        new XElement("step",
            new XAttribute("num", "4"),
            new XAttribute("task", "Кнопка Меню левого контроллера")),
        new XElement("step",
            new XAttribute("num", "5"),
            new XAttribute("task", "Кнопка Триггер правого контроллера")),
        new XElement("step",
            new XAttribute("num", "6"),
            new XAttribute("task", "Кнопка Триггер левого контроллера")),
        new XElement("step",
            new XAttribute("num", "7"),
            new XAttribute("task", "Тачпад правого контроллера")),
        new XElement("step",
            new XAttribute("num", "8"),
            new XAttribute("task", "Тачпад левого контроллера")),
        new XElement("step",
            new XAttribute("num", "9"),
            new XAttribute("task", "Кнопка Грип правого контроллера")),
        new XElement("step",
            new XAttribute("num", "10"),
            new XAttribute("task", "Кнопка Грип левого контроллера")))
        ));

        xdoc.Save(myXFilePathName);

    }



}
