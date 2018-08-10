using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using UnityEngine;


[XmlRoot("world")]
public class WorldData
{
    [XmlAttribute("worldRoot")]
    public string worldRoot;               // это имя объекта, в котором лежат все

    [XmlArray("controls"), XmlArrayItem("control")]
    public List<ControlData> controlsData = new List<ControlData>();

    public WorldData()
    {
    }

    public static WorldData Load(string path)
    {
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(WorldData));

        using (XmlReader myXmlRdr = XmlReader.Create(path))
        {
            return myXmlSrlzr.Deserialize(myXmlRdr) as WorldData;
        }
    }

    public void Save(string path)
    {
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(WorldData));
        XmlWriterSettings myXmlSettings = new XmlWriterSettings();
        myXmlSettings.Encoding = System.Text.Encoding.UTF8;             // Необязательно, т.к. используется по умолчанию
        myXmlSettings.Indent = true;

        using (XmlWriter myXmlWrtr = XmlWriter.Create(path, myXmlSettings))
        {
            myXmlSrlzr.Serialize(myXmlWrtr, this);
        }
    }

    public ControlData GetControlData(string path)
    {
        ControlData cd = controlsData.Find(x => x.nativePath == path);
        return cd;
    }



}
