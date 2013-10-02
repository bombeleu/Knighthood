using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof (NameListAttribute))]
public class NameListDrawer : PropertyDrawer
{
    private NameListAttribute nameListAttribute { get { return ((NameListAttribute) attribute); }}
    private bool foldout = false;
    private const float ITEMSIZE = 15f;


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldout)
        {
            return base.GetPropertyHeight(property, label) + ITEMSIZE * Enum.GetNames(nameListAttribute.names).Length;
        }
        else
        {
            return base.GetPropertyHeight(property, label);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        foldout = EditorGUI.Foldout(position, foldout, label);
        if (foldout)
        {
            //EditorGUI.BeginChangeCheck();
            //{
            //    Rect rect = EditorGUI.IndentedRect(position);
            //    int size = property.arraySize;
            //    size = EditorGUI.IntField(rect, size);
            //}
            //if (EditorGUI.EndChangeCheck())
            //{
            //    property.
            //}

            string[] names = Enum.GetNames(nameListAttribute.names);
            Rect rect = EditorGUI.IndentedRect(position);
            //rect.height = GetPropertyHeight()
            for (int i = 0; i < names.Length; i++)
            {

                rect.y += rect.height;
                //rect.height = 15f;
                EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(i), new GUIContent(i >= names.Length ? "" : names[i]));
            }
        }
    }
}
