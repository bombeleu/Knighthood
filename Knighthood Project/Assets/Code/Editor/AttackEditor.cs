// Steve Yeager
// 9.8.2013

using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor class for Attack.
/// </summary>
[CustomEditor(typeof(Attack))]
public class AttackEditor : Editor
{
    #region Private Fields

    private SerializedObject myObject;

    private SerializedProperty
        attackInput,
        attackName;

    #endregion


    #region Editor Overrides

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
            EditorGUILayout.LabelField(attackInput.stringValue, EditorStyles.boldLabel, GUILayout.MinWidth(0));
            EditorGUILayout.LabelField(attackName.stringValue, EditorStyles.boldLabel, GUILayout.MinWidth(0));
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion
}