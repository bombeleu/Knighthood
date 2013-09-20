using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class RegexAttribute : PropertyAttribute
{
    public readonly string pattern;
    public readonly string helpMessage;

    public RegexAttribute(string pattern, string helpMessage)
    {
        this.pattern = pattern;
        this.helpMessage = helpMessage;
    }
}


[CustomPropertyDrawer(typeof(RegexAttribute))]
public class RegexDrawer : PropertyDrawer
{
    private const int helpHeight = 30;
    private const int textHeight = 16;

    RegexAttribute regexAttribute {get { return ((RegexAttribute) attribute); }}


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsValid(property))
        {
            return base.GetPropertyHeight(property, label);
        }
        else
        {
            return base.GetPropertyHeight(property, label) + helpHeight;
        }
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect textFieldPosition = position;
        textFieldPosition.height = textHeight;
        DrawTextField(textFieldPosition, property, label);

        Rect helpPosition = EditorGUI.IndentedRect(position);
        helpPosition.y += textHeight;
        helpPosition.height = helpHeight;
        DrawHelpBox(helpPosition, property);
    }


    bool IsValid(SerializedProperty prop)
    {
        return Regex.IsMatch(prop.stringValue, regexAttribute.pattern);
    }


    void DrawTextField(Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        string value = EditorGUI.TextField(position, label, prop.stringValue);
        if (EditorGUI.EndChangeCheck())
        {
            prop.stringValue = value;
        }
    }


    void DrawHelpBox(Rect position, SerializedProperty prop)
    {
        if (IsValid(prop)) return;
        EditorGUI.HelpBox(position, regexAttribute.helpMessage, MessageType.Error);
    }
}


public class RegexExample : MonoBehaviour
{
    [Regex(@"^(?:\d{1,3}\.){3}\d{1,3}$", "Invalid IP address!\nExample: '127.0.0.1'")]
    public string serverAddress = "192.168.0.1";

    //public ScaledCurve curve;
}