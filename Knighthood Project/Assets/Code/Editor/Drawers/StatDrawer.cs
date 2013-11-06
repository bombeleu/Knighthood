// Steve Yeager
// 11.3.2013

using UnityEngine;
using System.Collections;
using UnityEditor;

//[CustomPropertyDrawer(typeof(Stat))]
public class StatDrawer : PropertyDrawer
{
    StatAttribute myAttribute { get { return (StatAttribute)attribute; } }


    #region PropertyDrawer Overrides

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
        //EditorGUI.indentLevel++;
        //EditorGUI.PropertyField(position, property.FindPropertyRelative("levelValue"), new GUIContent("Level"));
        //EditorGUI.indentLevel--;
        EditorGUI.BeginProperty(position, label, property);
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            //EditorGUI.PropertyField(new Rect(position.x, position.y, 30, position.height), property, GUIContent.none, true);
           
            EditorGUI.indentLevel = indent;
        }
        EditorGUI.EndProperty();
    }

    #endregion
}