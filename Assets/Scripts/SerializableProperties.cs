using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class SerializableProperties : MonoBehaviour
//{
//}

[System.Serializable]
public class SerializableColor
{

    public float[] colorStore = { 1F, 1F, 1F, 1F }; //new float[4]
    public Color Color
    {
        get { return new Color(colorStore[0], colorStore[1], colorStore[2], colorStore[3]); }
        set { colorStore = new float[4] { value.r, value.g, value.b, value.a }; }
    }

    //makes this class usable as Color, Color normalColor = mySerializableColor;
    public static implicit operator Color(SerializableColor instance)
    {
        return instance.Color;
    }

    //makes this class assignable by Color, SerializableColor myColor = Color.white;
    public static implicit operator SerializableColor(Color color)
    {
        return new SerializableColor { Color = color };
    }
}

[System.Serializable]
public class SerializableVector2
{

    public float[] vectorStore = { 1F, 1F }; //new float[2]
    public Vector2 Vector2
    {
        get { return new Vector2(vectorStore[0], vectorStore[1]); }
        set { vectorStore = new float [2] { value.x, value.y }; }
    }

    public static implicit operator Vector2(SerializableVector2 instance)
    {
        return instance.Vector2;
    }

    public static implicit operator SerializableVector2(Vector2 vector2)
    {
        return new SerializableVector2 { Vector2 = vector2 }; //creating instance while invoking the setter
    } 
}