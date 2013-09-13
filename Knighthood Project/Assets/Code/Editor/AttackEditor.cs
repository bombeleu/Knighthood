// Steve Yeager
// 9.8.2013

using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(Attack))]
public class AttackEditor : Editor
{
    private SerializedObject myObject;

    private SerializedProperty
        attackInput,
        attackName;


    protected virtual void OnEnable()
    {
        myObject = new SerializedObject(target);

        attackInput = myObject.FindProperty("attackInput");
        attackName = myObject.FindProperty("attackName");
    }


    public override void OnInspectorGUI()
    {
        myObject.Update();

        EditorGUILayout.LabelField("Base", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(attackInput.stringValue, EditorStyles.whiteBoldLabel, GUILayout.MinWidth(0));
            EditorGUILayout.LabelField(attackName.stringValue, EditorStyles.whiteBoldLabel, GUILayout.MinWidth(0));
        }
        EditorGUILayout.EndHorizontal();
    }
}