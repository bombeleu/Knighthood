// Steve Yeager
// 9.7.2013

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor for ComboManager.
/// </summary>
[CustomEditor(typeof(ComboManager))]
public class ComboManagerEditor : Editor
{
    #region Reference Fields

    private ComboManager manager;

    #endregion

    #region Serialized Fields

    private SerializedObject managerSerialized;

    private SerializedProperty
        attacks,
        inputs,
        comboStrings,
        open;

    #endregion

    #region Private Fields

    private int confirmDelete = -1;
    private bool confirmDeleteAll;
    private bool confirmAdd;
    private Type[] attackScripts;
    private const string ScriptPath = "Assets/Code/Game/Combat/Attacks";
    private const float ButtonSize = 30;
    private int moveIndex = -1;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        managerSerialized = new SerializedObject(target);
        manager = (ComboManager)target;

        attacks = managerSerialized.FindProperty("attacks");
        inputs = managerSerialized.FindProperty("inputs");
        comboStrings = managerSerialized.FindProperty("comboStrings");
        open = managerSerialized.FindProperty("open");

        attackScripts = LoadAttackScripts();
        confirmAdd = false;
    }


    public override void OnInspectorGUI()
    {
        managerSerialized.Update();

        #region Inputs

        EditorGUILayout.LabelField("Inputs");

        // all inputs
        for (int i = 0; i < inputs.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(inputs.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(0));

                // delete
                if (GUILayout.Button("-", GUILayout.Width(ButtonSize)))
                {
                    confirmDelete = i;
                }
            }
            EditorGUILayout.EndHorizontal();

            // confirm delete
            if (confirmDelete == i)
            {
                if (GUILayout.Button("Delete " + inputs.GetArrayElementAtIndex(i).stringValue))
                {
                    inputs.DeleteArrayElementAtIndex(i);
                    managerSerialized.ApplyModifiedProperties();
                    confirmDelete = -1;
                    return;
                }
            }
        }

        // add new
        if (GUILayout.Button("Add Input"))
        {
            inputs.InsertArrayElementAtIndex(inputs.arraySize);
            managerSerialized.ApplyModifiedProperties();
            manager.inputs[manager.inputs.Length - 1] = "";
            return;
        }
        #endregion

        GUILayout.Space(ButtonSize/2);

        #region Attacks Title

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Combo", GUILayout.MinWidth(0));
            EditorGUILayout.LabelField("Name", GUILayout.MinWidth(0));

            // close all
            if (GUILayout.Button("||", GUILayout.Width(ButtonSize)))
            {
                for (int i = 0; i < open.arraySize; i++)
                {
                    open.GetArrayElementAtIndex(i).boolValue = false;
                }
            }

            // delete all
            if (GUILayout.Button("--", GUILayout.Width(ButtonSize)) && attacks.arraySize > 0)
            {
                confirmDeleteAll = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        #endregion

        #region Delete All

        if (confirmDeleteAll)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // confirm delete all
                if (GUILayout.Button("Delete All"))
                {
                    while (attacks.arraySize > 0)
                    {
                        Delete(0);
                    }
                    return;
                }

                // cancel
                if (GUILayout.Button("Cancel"))
                {
                    confirmDeleteAll = false;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Attacks

        for (int i = 0; i < attacks.arraySize; i++)
        {
            if (DrawAttack(i))
            {
                return;
            }

            manager.attacks[i].hideFlags = open.GetArrayElementAtIndex(i).boolValue ? HideFlags.None : HideFlags.HideInInspector;
        }

        #endregion

        #region New Attack

        if (confirmAdd)
        {
            foreach (var type in attackScripts)
            {
                if (GUILayout.Button(Regex.Replace(type.Name, "(\\B[A-Z])", " $1")))
                {
                    AddAttack(type);
                    return;
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                confirmAdd = false;
            }
        }
        // add new attackValue
        else
        {
            if (GUILayout.Button("Add New Attack"))
            {
                confirmAdd = true;
            }
        }

        #endregion

        // confirm delete all

        managerSerialized.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void Delete(int index)
    {
        DestroyImmediate(manager.attacks[index], true);
        attacks.DeleteArrayElementAtIndex(index);
        attacks.DeleteArrayElementAtIndex(index);
        inputs.DeleteArrayElementAtIndex(index);
        open.DeleteArrayElementAtIndex(index);

        confirmDelete = -1;
        confirmDeleteAll = false;
        managerSerialized.ApplyModifiedProperties();
    }


    private void AddAttack(Type attack, int index = -1)
    {
        if (index == -1)
        {
            index = attacks.arraySize;
        }

        attacks.InsertArrayElementAtIndex(index);
        comboStrings.InsertArrayElementAtIndex(index);
        open.InsertArrayElementAtIndex(index);
        open.GetArrayElementAtIndex(index).boolValue = true;
        managerSerialized.ApplyModifiedProperties();
        manager.attacks[index] = manager.gameObject.AddComponent(attack.Name) as Attack;
        manager.attacks[index].attackInput = "";
        manager.attacks[index].attackName = "";

        open.InsertArrayElementAtIndex(index);
    }


    private static Type[] LoadAttackScripts()
    {
        string[] scriptNames = Directory.GetFiles(ScriptPath, "*.cs", SearchOption.AllDirectories);
        List<Type> types = new List<Type>();
        foreach (var script in scriptNames)
        {
            MonoScript mono = (MonoScript)AssetDatabase.LoadAssetAtPath(script, typeof(MonoScript));
            if (mono.GetClass() != null && mono.GetClass().IsSubclassOf(typeof(Attack)))
            {
                types.Add(mono.GetClass());
            }
        }
        return types.ToArray();
    }


    private bool DrawAttack(int index)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PropertyField(comboStrings.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.MinWidth(0));
            managerSerialized.ApplyModifiedProperties();
            manager.attacks[index].attackInput = comboStrings.GetArrayElementAtIndex(index).stringValue;

            SerializedObject attack = new SerializedObject(manager.attacks[index]);
            EditorGUILayout.PropertyField(attack.FindProperty("attackName"), GUIContent.none, GUILayout.MinWidth(0));
            attack.ApplyModifiedProperties();

            // move
            if (moveIndex == index)
            {
                if (GUILayout.Button("X", GUILayout.Width(ButtonSize)))
                {
                    moveIndex = -1;
                }
            }
            else if (moveIndex != -1)
            {
                if (GUILayout.Button("T", GUILayout.Width(ButtonSize)))
                {
                    attacks.MoveArrayElement(moveIndex, index);
                    comboStrings.MoveArrayElement(moveIndex, index);
                    open.MoveArrayElement(moveIndex, index);
                    managerSerialized.ApplyModifiedProperties();
                    moveIndex = -1;
                    return true;
                }
            }
            else
            {
                if (GUILayout.Button("M", GUILayout.Width(ButtonSize)))
                {
                    moveIndex = index;
                }
            }

            // delete
            if (GUILayout.Button("-", GUILayout.Width(ButtonSize)))
            {
                confirmDelete = index;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (confirmDelete == index)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // delete
                if (GUILayout.Button("Delete"))
                {
                    Delete(index);
                    return true;
                }
                // cancel
                if (GUILayout.Button("Cancel"))
                {
                    confirmDelete = -1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        return false;
    }

    #endregion
}