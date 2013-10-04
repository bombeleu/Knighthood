using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class NameListAttribute : PropertyAttribute
{
    public readonly Type names;
    public readonly IEnumerable list;

    //public NameListAttribute(Type names)
    //{
    //    this.names = names;
    //}


    public NameListAttribute(object obj, string listName)
    {
        list = (IEnumerable)obj.GetType().GetProperty(listName).GetValue(obj, null);
    }
}