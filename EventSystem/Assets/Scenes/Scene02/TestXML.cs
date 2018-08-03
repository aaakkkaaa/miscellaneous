using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class TestXML : MonoBehaviour {

    // Имя папки для файлов данных
    private string myDataDirName = "Data";

    // Полное имя папки для файлов данных (с путем)
    private string myRecDir;

    // Имя файла файлов данных
    private string myTestFileName = "QQ.xml";

    //[Serializable]
    //[XmlInclude(typeof(TestXML))]
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Company Company { get; set; }

        public Person()
        { }

        public Person(string name, int age, Company comp)
        {
            Name = name;
            Age = age;
            Company = comp;
        }
    }

    //[Serializable]
    //[XmlInclude(typeof(TestXML))]
    public class Company
    {
        public string Name { get; set; }

        // стандартный конструктор без параметров
        public Company() { }

        public Company(string name)
        {
            Name = name;
        }
    }

    // Use this for initialization
    void Start () {

        // Папка данных. Работаем от корня текущей папки проекта

        // Directory.CreateDirectory(myDataDirName); // если нужно будет, то создадим

        // Правильное имя для полного пути к папке
        myRecDir = Path.Combine(Directory.GetCurrentDirectory(), myDataDirName);

        // Файлы в папке
        string[] myFiles = Directory.GetFiles(myRecDir, "*.*");
        foreach (string myFile in myFiles)
        {
            print(myFile);
        }
        print("Всего файлов: " + myFiles.Length);

        // Правильное имя для полного пути к файлу
        string myTestFile = Path.Combine(myRecDir, myTestFileName);


        Person person1 = new Person("Tom", 29, new Company("Microsoft"));
        Person person2 = new Person("Bill", 25, new Company("Apple"));
        Person[] people = new Person[] { person1, person2 };


        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(Person[]));

        //using (FileStream fs = new FileStream(myTestFile, FileMode.OpenOrCreate))

        XmlWriterSettings myXmlSettings = new XmlWriterSettings();
        //print("myXmlSettings.Encoding = " + myXmlSettings.Encoding);

        myXmlSettings.Encoding = System.Text.Encoding.UTF8; // Необязательно, т.к. используется по умолчанию
        myXmlSettings.Indent = true;


        using (XmlWriter myXmlWrtr = XmlWriter.Create(myTestFile, myXmlSettings))
        {
            myXmlSrlzr.Serialize(myXmlWrtr, people);
        }


        //using (XmlReader myXmlRdr = XmlReader.Create(myTestFile))
        //{
        //    Person myXmlObj = (Person)myXmlSrlzr.Deserialize(myXmlRdr);
        //    print(myXmlObj.Name + " " + myXmlObj.Age + " " + myXmlObj.Company.Name);
        //}






    }

    // Update is called once per frame
    void Update () {
		
	}
}
