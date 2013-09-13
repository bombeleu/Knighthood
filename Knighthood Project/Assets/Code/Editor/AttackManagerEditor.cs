// Steve Yeager
// 9.7.2013

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(AttackManager))]
public class AttackManagerEditor : Editor
{
    #region Reference Fields

    private AttackManager manager;

    #endregion

    #region Serialized Fields

    private SerializedObject managerSerialized;

    private SerializedProperty
        attacks,
        inputs,
        open;

    #endregion

    #region Private Fields

    private int confirmDelete = -1;
    private bool confirmDeleteAll;
    private bool confirmAdd;
    private Type[] attackScripts;
    private const string ScriptPath = "Assets/Code/Game/Combat/Attacks";
    private const float ButtonSize = 30;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        managerSerialized = new SerializedObject(target);
        manager = (AttackManager)target;

        attacks = managerSerialized.FindProperty("attacks");
        inputs = managerSerialized.FindProperty("inputs");
        open = managerSerialized.FindProperty("open");

        attackScripts = LoadAttackScripts();
        confirmAdd = false;
    }


    public override void OnInspectorGUI()
    {
        managerSerialized.Update();

        #region  Title

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Attack Input", GUILayout.MinWidth(0));
            EditorGUILayout.LabelField("Attack Name", GUILayout.MinWidth(0));

            // close all
            if (GUILayout.Button(new GUIContent(@"\", "Close All"), GUILayout.Width(30)))
            {
                for (int i = 0; i < open.arraySize; i++)
                {
                    open.GetArrayElementAtIndex(i).boolValue = false;
                }
            }

            // delete all
            if (GUILayout.Button(new GUIContent("--", "Delete All"), GUILayout.Width(30)) && attacks.arraySize > 0)
            {
                confirmDeleteAll = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        #endregion

        #region Attacks

        for (int i = 0; i < manager.attacks.Length; i++)
        {
            if (DrawAttack(i))
            {
                return;
            }

            manager.attacks[i].hideFlags = open.GetArrayElementAtIndex(i).boolValue ? HideFlags.None : HideFlags.HideInInspector;
        }

        #endregion

        #region Delete All

        if (confirmDeleteAll)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // delete all
                if (GUILayout.Button("Delete All Attacks"))
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
        // add new attack
        else
        {
            if (GUILayout.Button("Add New Attack"))
            {
                confirmAdd = true;
            }
        }

        #endregion

        managerSerialized.ApplyModifiedProperties();
    }


    //public override void OnInspectorGUI()
    //{
    //    managerSerialized.Update();
    //    //base.OnInspectorGUI();

    //    #region titles

    //    EditorGUILayout.BeginHorizontal();
    //    {
    //        EditorGUILayout.LabelField("Attack Input", GUILayout.MinWidth(0));
    //        EditorGUILayout.LabelField("Attack Name", GUILayout.MinWidth(0));

    //        // close all
    //        if (GUILayout.Button(new GUIContent(@"\", "Close All"), GUILayout.Width(30)))
    //        {
    //            for (int i = 0; i < open.arraySize; i++)
    //            {
    //                open.GetArrayElementAtIndex(i).boolValue = false;
    //            }
    //        }

    //        // delete all
    //        if (GUILayout.Button(new GUIContent("--", "Delete All"), GUILayout.Width(30)))
    //        {
    //            confirmDeleteAll = true;
    //        }
    //    }
    //    EditorGUILayout.EndHorizontal();

    //    // confirm delete all
    //    if (confirmDeleteAll)
    //    {
    //        EditorGUILayout.BeginHorizontal();
    //        {
    //            if (GUILayout.Button("Delete All"))
    //            {
    //                while (attacks.arraySize > 0)
    //                {
    //                    Delete(0);
    //                }
    //                confirmDeleteAll = false;
    //                return;
    //            }

    //            if (GUILayout.Button("Cancel"))
    //            {
    //                confirmDeleteAll = false;
    //            }
    //        }
    //        EditorGUILayout.EndHorizontal();
    //    }

    //    #endregion

    //    #region all attacks

    //    EditorGUILayout.BeginVertical();
    //    for (int i = 0; i < attacks.arraySize; i++)
    //    {
    //        bool altInput = altInputs.GetArrayElementAtIndex(i).boolValue;

    //        // attack info
    //        EditorGUILayout.BeginHorizontal();
    //        {
    //            EditorGUILayout.PropertyField(genericAttackTypes.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(0));
    //            manager.attacks[i].attackInput = manager.attackInputs[i];

    //            // not changable if another input for a combo
    //            if (altInput)
    //            {
    //                EditorGUILayout.LabelField(attackNames.GetArrayElementAtIndex(i).stringValue, GUILayout.MinWidth(0));
    //            }
    //            else
    //            {
    //                EditorGUILayout.PropertyField(attackNames.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(0));
    //                manager.attacks[i].attackName = manager.attackNames[i];
    //            }

    //            // add input to combo
    //            if (manager.attacks[i].GetType() == typeof(ComboAttack) && !altInput)
    //            {
    //                if (GUILayout.Button(new GUIContent("+", "Add input"), GUILayout.Width(30)))
    //                {
    //                    AddInput(i + 1);
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                GUILayout.Button("", GUILayout.Width(30));
    //            }

    //            // open/close attack
    //            if (altInput)
    //            {
    //                GUILayout.Button("", GUILayout.Width(30));
    //            }
    //            else
    //            {
    //                if (GUILayout.Button(new GUIContent(open.GetArrayElementAtIndex(i).boolValue ? "|" : "O", open.GetArrayElementAtIndex(i).boolValue ? "Close" : "Open"), GUILayout.Width(30)))
    //                {
    //                    open.GetArrayElementAtIndex(i).boolValue = !open.GetArrayElementAtIndex(i).boolValue;
    //                }
    //            }

    //            // delete attack
    //            if (GUILayout.Button(new GUIContent("-", "Delete"), GUILayout.Width(30)))
    //            {
    //                if (altInput)
    //                {
    //                    Delete(i);
    //                    return;
    //                }
    //                else
    //                {
    //                    confirmDelete = i;   
    //                }
    //            }
    //        }
    //        EditorGUILayout.EndHorizontal();

    //        // confirm delete
    //        if (confirmDelete == i)
    //        {
    //            EditorGUILayout.BeginHorizontal();
    //            {
    //                if (GUILayout.Button("Delete"))
    //                {
    //                    Delete(i);
    //                    return;
    //                }

    //                if (GUILayout.Button("Cancel"))
    //                {
    //                    confirmDelete = -1;
    //                }
    //            }
    //            EditorGUILayout.EndHorizontal();
    //        }

    //        // hide/show attack
    //        if (!altInput)
    //        {
    //            manager.attacks[i].hideFlags = (open.GetArrayElementAtIndex(i).boolValue ? HideFlags.None : HideFlags.HideInInspector);
    //        }
    //    }
    //    EditorGUILayout.EndVertical();

    //    // confirm add
    //    if (confirmAdd)
    //    {
    //        foreach (var type in attackScripts)
    //        {
    //            if (GUILayout.Button(Regex.Replace(type.Name, "(\\B[A-Z])", " $1")))
    //            {
    //                AddAttack(type);
    //            }
    //        }

    //        if (GUILayout.Button("Cancel"))
    //        {
    //            confirmAdd = false;
    //        }
    //    }
    //    // add new attack
    //    else
    //    {
    //        if (GUILayout.Button("Add New Attack"))
    //        {
    //            confirmAdd = true;
    //        }
    //    }

    //    #endregion

    //    managerSerialized.ApplyModifiedProperties();
    //}

    #endregion

    #region Private Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="index"></param>
    private void AddAttack(Type attack, int index = -1)
    {
        if (index == -1)
        {
            index = attacks.arraySize;
        }

        attacks.InsertArrayElementAtIndex(index);
        inputs.InsertArrayElementAtIndex(index);
        open.InsertArrayElementAtIndex(index);
        open.GetArrayElementAtIndex(index).boolValue = true;
        managerSerialized.ApplyModifiedProperties();
        manager.attacks[index] = manager.gameObject.AddComponent(attack.Name) as Attack;
        manager.attacks[index].attackInput = "";
        manager.attacks[index].attackName = "";

        open.InsertArrayElementAtIndex(index);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool DrawAttack(int index)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PropertyField(inputs.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.MinWidth(0));
            managerSerialized.ApplyModifiedProperties();
            manager.attacks[index].attackInput = inputs.GetArrayElementAtIndex(index).stringValue;

            SerializedObject attack = new SerializedObject(manager.attacks[index]);
            EditorGUILayout.PropertyField(attack.FindProperty("attackName"), GUIContent.none, GUILayout.MinWidth(0));
            attack.ApplyModifiedProperties();

            // toggle
            if (GUILayout.Button(manager.open[index] ? "|" : "O", GUILayout.Width(ButtonSize)))
            {
                manager.open[index] = !manager.open[index];
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