using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TestLINQ : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        XDocument xdoc = new XDocument(new XElement("topic", new XAttribute("title", "Освоение тренажера"),
            new XElement("part",
                new XAttribute("name", "Индивидуальная настройка маски VR"),
                new XElement("step", new XAttribute("number", "1"), new XElement("task", "Понять устройство маски VR")),
                new XElement("step", new XAttribute("number", "2"), new XElement("task", "Настроить маску под курсанта")),
            new XElement("part",
                new XElement("step", new XAttribute("number", "1"), new XElement("task", "Первый опыт в витруальном пространстве")),
                new XElement("step", new XAttribute("number", "2"), new XElement("task", "Понять, как отображаются в виртуальном пространстве границы рабочей зоны")),
                new XElement("price", "33000")))));
        xdoc.Save("phones.xml");
        
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
		
	}
}
