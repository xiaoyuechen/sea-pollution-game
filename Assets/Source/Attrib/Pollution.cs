﻿[System.Serializable]
public class Pollution
{
    public enum Type
    {
        TYPE_1,
        TYPE_2,
        TYPE_3,
        TYPE_4,
        TYPE_5,
        TYPE_6,
        TYPE_7,
        TYPE_8,
        TYPE_9,
        TYPE_10,
    }

    [System.NonSerialized] public Type type = Type.TYPE_1;
    public string title = "default pollution";
}
