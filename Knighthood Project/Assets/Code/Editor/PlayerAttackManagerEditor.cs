// Steve Yeager
// 9.2.2013

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(PlayerAttackManager))]
public class PlayerAttackManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedObject player = new SerializedObject(target);
        SerializedProperty attacks = player.FindProperty("attacks");
        string[] attackTypes = Enum.GetNames(typeof(PlayerAttackManager.AttacksTypes));
        player.Update();

        for (int i = 0; i < attackTypes.Length-1; i++)
        {
            EditorGUILayout.PropertyField(attacks.GetArrayElementAtIndex(i), new GUIContent(attackTypes[i]));
        }

        player.ApplyModifiedProperties();
    } // end OnInspectorGUI

} // end PlayerAttackManagerEditor class