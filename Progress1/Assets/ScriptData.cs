using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("step")]
public class ScriptData
{
    public ScriptData() { }

    [XmlAttribute("world")]
    public string worldFileName;
    [XmlArray("actions"), XmlArrayItem("action")]
    public List<ActionData> actions = new List<ActionData>();

    public static ScriptData Load(string path)
    {
        XmlSerializer myXmlSrlzr = new XmlSerializer(typeof(ScriptData));

        using (XmlReader myXmlRdr = XmlReader.Create(path))
        {
            return myXmlSrlzr.Deserialize(myXmlRdr) as ScriptData;
        }
    }
}

[Serializable]
public class ActionData
{
    public ActionData() { }
    [XmlAttribute("num")]
    public int num;
}


