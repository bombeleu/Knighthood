using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class NameListAttribute : PropertyAttribute
{
    public readonly Type enumType;

    public NameListAttribute(Type enumType)
    {
        this.enumType = enumType;
    }
}