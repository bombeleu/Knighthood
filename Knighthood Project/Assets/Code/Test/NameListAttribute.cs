using System;
using UnityEditor;
using UnityEngine;

public class NameListAttribute : PropertyAttribute
{
    public readonly Type names;

    public NameListAttribute(Type names)
    {
        this.names = names;
    }
}