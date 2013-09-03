// Steve Yeager
// 8.21.2013

using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(PlayerAttacks))]
public class PlayerAttacksEditor : Editor
{
    #region Toggle Fields

    private bool lightToggle = false;
    private bool heavyToggle = false;
    private bool rangedToggle = false;
    private bool magicToggle = false;

    #endregion


    public override void OnInspectorGUI()
    {
        SerializedObject player = new SerializedObject(target);
        player.Update();

        lightToggle = EditorGUILayout.Foldout(lightToggle, "Light Attacks");
        if (lightToggle)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(0), new GUIContent("Light Normal"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(1), new GUIContent("Light Side"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(2), new GUIContent("Light Up"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(3), new GUIContent("Light Down"));
            EditorGUI.indentLevel--;
        }
        heavyToggle = EditorGUILayout.Foldout(heavyToggle, "Heavy Attacks");
        if (heavyToggle)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(4), new GUIContent("Heavy Normal"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(5), new GUIContent("Heavy Side"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(6), new GUIContent("Heavy Up"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(7), new GUIContent("Heavy Down"));
        }
        rangedToggle = EditorGUILayout.Foldout(rangedToggle, "Ranged Attacks");
        if (rangedToggle)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(8), new GUIContent("Ranged Normal"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(9), new GUIContent("Ranged Side"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(10), new GUIContent("Ranged Up"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(11), new GUIContent("Ranged Down"));
        }
        magicToggle = EditorGUILayout.Foldout(magicToggle, "Magic Attacks");
        if (magicToggle)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(12), new GUIContent("Magic Left"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(13), new GUIContent("Magic Up"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(14), new GUIContent("Magic Right"));
            EditorGUILayout.PropertyField(player.FindProperty("attackSystems").GetArrayElementAtIndex(15), new GUIContent("Magic Down"));
        }


        player.ApplyModifiedProperties();
    } // end OnInspectorGUI

} // end PlayerAttackSystemsEditor class