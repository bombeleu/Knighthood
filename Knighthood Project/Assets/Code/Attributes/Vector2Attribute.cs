// Steve Yeager
// 9.7.2013

using UnityEditor;
using UnityEngine;

/// <summary>
/// Holds data for X and Y labels.
/// </summary>
public class Vector2Attribute : PropertyAttribute
{
    public readonly string xLabel;
    public readonly string yLabel;


    public Vector2Attribute(string xLabel = "X", string yLabel = "Y")
    {
        this.xLabel = xLabel;
        this.yLabel = yLabel;
    }

}


[CustomPropertyDrawer(typeof (Vector2Attribute))]
public class Vector2Drawer : PropertyDrawer
{
    Vector2Attribute vector2Attribute {get { return ((Vector2Attribute) attribute); }}


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Debug.Log(position);
        ////EditorGUI.LabelField(new Rect(0, position.y, position.width/2, position.height), label);
        //EditorGUILayout.LabelField(label, GUILayout.Width(80));
        //GUILayout.FlexibleSpace();
        //EditorGUILayout.FloatField(vector2Attribute.xLabel, property.FindPropertyRelative("x").floatValue);
        //int indent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 0;
        ////EditorGUI.FloatField(new Rect(, position.y, position.width/4, position.height), vector2Attribute.xLabel, property.FindPropertyRelative("x").floatValue);
        ////EditorGUI.FloatField(new Rect(position.width - position.width/ 4, position.y, position.width / 4, position.height), vector2Attribute.yLabel, property.FindPropertyRelative("y").floatValue);
        //EditorGUI.indentLevel = indent;

        //EditorGUILayout.BeginHorizontal();
        //{
        //    EditorGUILayout.LabelField(label, GUILayout.Width(100));
        //    int indent = EditorGUI.indentLevel;
        //    EditorGUI.indentLevel = 0;
        //    EditorGUILayout.FloatField(vector2Attribute.xLabel, property.FindPropertyRelative("x").floatValue, GUILayout.MinWidth(50f));
        //    //EditorGUILayout.FloatField(vector2Attribute.yLabel, property.FindPropertyRelative("y").floatValue, GUILayout.MaxWidth(50f));
        //    EditorGUI.indentLevel = indent;
        //}
        //EditorGUILayout.EndHorizontal();



        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect xRect = new Rect(position.x, position.y, 50, position.height);
        Rect yRect = new Rect(position.x + 50, position.y, 50, position.height);

        EditorGUI.FloatField(xRect, "X", property.FindPropertyRelative("x").floatValue);
        EditorGUI.FloatField(yRect, "Y", property.FindPropertyRelative("y").floatValue);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}