using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class ControlData
{
    [XmlAttribute("nativePath")]
    public string nativePath;               // место в иерархии в исходном состоянии
    [XmlAttribute("currentPath")]
    public string currentPath;              // место в иерархии, в котором объект оказался на момент сохранения данных
    [XmlAttribute("active")]
    public bool active;                     // состояние активности gameObject

    public Vec3 pos;
    public Vec3 rot;
    public Vec3 scale;

    public State state;

    public ControlData() { }

    // считывание-установка параметров transform
    public void SetPos(Vector3 v)
    {
        pos = new Vec3(v);
    }
    public Vector3 GetPos()
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }

    public void SetRot(Vector3 v)
    {
        rot = new Vec3(v);
    }
    public Vector3 GetRot()
    {
        return new Vector3(rot.x, rot.y, rot.z);
    }

    public void SetScale(Vector3 v)
    {
        if( v.x != 1.0f && v.y != 1.0f && v.z != 1.0f )
        {
            scale = new Vec3(v);
        }
        else
        {
            scale = null;
        }
    }
    public Vector3 GetScale()
    {
        if( scale != null )
        { 
            return new Vector3(scale.x, scale.y, scale.z);
        }
        else
        {
            return new Vector3(1, 1, 1);
        }
    }

    public void SetState()
    {

    }

}

[Serializable]
public class Vec3
{
    public Vec3() { }
    public Vec3(float newX, float newY, float newZ)
    {
        x = newX;
        y = newY;
        z = newZ;
    }
    public Vec3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    [XmlAttribute("x")]
    public float x;
    [XmlAttribute("y")]
    public float y;
    [XmlAttribute("z")]
    public float z;
}

[Serializable]
public class State
{
    public State()
    {
        freeState = "";
        openState = "";
        downState = "";
        param = -1.0f;
    }
    public State(string fs, string os="", float  p=-1.0f)
    {
        freeState = fs;
        openState = os;
        param = p;
    }
    [XmlAttribute("freeState")]
    public string freeState;    //fixed, free, hand_r, hand_l,  не импользуем ""
    [XmlAttribute("openState")]
    public string openState;    //close, ajar, open,  не импользуем ""
    [XmlAttribute("downState")]
    public string downState;    //up, down
    [XmlAttribute("param")]
    public float param;         // 0..100,  не импользуем -1
}
