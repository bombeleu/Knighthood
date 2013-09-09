// Steve Yeager
// 9.7.2013

using System.Collections.Generic;
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
        genericAttacks,
        genericAttackTypes,
        attackNames,
        open;

    #endregion

    #region Private Fields

    private int confirmDelete = -1;
    private bool confirmDeleteAll;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        managerSerialized = new SerializedObject(target);
        manager = (AttackManager)target;

        genericAttacks = managerSerialized.FindProperty("genericAttacks");
        genericAttackTypes = managerSerialized.FindProperty("genericAttackTypes");
        attackNames = managerSerialized.FindProperty("attackNames");
        open = managerSerialized.FindProperty("open");
    }


    public override void OnInspectorGUI()
    {
        managerSerialized.Update();

        #region titles
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Attack Type", GUILayout.MinWidth(0));
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
            if (GUILayout.Button(new GUIContent("--", "Delete All"), GUILayout.Width(30)))
            {
                confirmDeleteAll = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (confirmDeleteAll)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Delete All"))
                {
                    while (genericAttacks.arraySize > 0)
                    {
                        Delete(0);
                    }
                    confirmDeleteAll = false;
                    return;
                }

                if (GUILayout.Button("Cancel"))
                {
                    confirmDeleteAll = false;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region all attacks

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < genericAttacks.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(genericAttackTypes.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(0));
                manager.genericAttacks[i].attackType = manager.genericAttackTypes[i];
                EditorGUILayout.PropertyField(attackNames.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.MinWidth(0));
                manager.genericAttacks[i].attackName = manager.attackNames[i];


                if (GUILayout.Button(open.GetArrayElementAtIndex(i).boolValue ? "|" : "O", GUILayout.Width(30)))
                {
                    open.GetArrayElementAtIndex(i).boolValue = !open.GetArrayElementAtIndex(i).boolValue;
                }
                if (GUILayout.Button(new GUIContent("-", "Delete"), GUILayout.Width(30)))
                {
                    confirmDelete = i;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (confirmDelete == i)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Delete"))
                    {
                        Delete(i);
                        return;
                    }

                    if (GUILayout.Button("Cancel"))
                    {
                        confirmDelete = -1;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // hide/show attack
            manager.genericAttacks[i].hideFlags = (open.GetArrayElementAtIndex(i).boolValue ? HideFlags.None : HideFlags.HideInInspector);
        }
        EditorGUILayout.EndVertical();

        // add new attack
        if (GUILayout.Button("Add New Attack"))
        {
            int index = genericAttacks.arraySize;

            genericAttacks.InsertArrayElementAtIndex(index);
            genericAttackTypes.InsertArrayElementAtIndex(index);
            attackNames.InsertArrayElementAtIndex(index);
            open.InsertArrayElementAtIndex(index);
            open.GetArrayElementAtIndex(index).boolValue = true;


            managerSerialized.ApplyModifiedProperties();
            manager.genericAttacks[index] = manager.gameObject.AddComponent<GenericAttack>();

            manager.genericAttackTypes[index] = "";
            manager.attackNames[index] = "";
        }

        #endregion

        managerSerialized.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void Delete(int i)
    {
        DestroyImmediate(manager.genericAttacks[i], true);
        genericAttacks.DeleteArrayElementAtIndex(i);
        genericAttacks.DeleteArrayElementAtIndex(i);
        genericAttackTypes.DeleteArrayElementAtIndex(i);
        attackNames.DeleteArrayElementAtIndex(i);
        open.DeleteArrayElementAtIndex(i);

        confirmDelete = -1;
        managerSerialized.ApplyModifiedProperties();
    }

    #endregion

}