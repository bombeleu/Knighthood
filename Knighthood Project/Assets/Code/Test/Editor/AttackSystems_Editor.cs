// Steve Yeager
// 

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

/// <summary>
/// 
/// </summary>
//[CustomEditor(typeof(AttackSystems))]
public class AttackSystems_Editor : Editor
{
    SerializedObject manager;
    AttackSystems Instance;
    SerializedProperty attackSystems;
    private int newAttack = -1;
    private bool[] toggles = new bool[16];



    public void OnEnable()
    {
        manager = new SerializedObject(target);
        Instance = (AttackSystems)target;
        attackSystems = manager.FindProperty("attackSystems");
    } // end OnEnable


    public override void OnInspectorGUI()
    {
        manager.Update();

        for (int i = 0; i < attackSystems.arraySize; i++)
        {
            DisplayAttackSystem(i);
        }

        manager.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    } // end OnInspectorGUI


    private void DisplayAttackSystem(int i)
    {
        SerializedProperty abilitySystem = attackSystems.GetArrayElementAtIndex(i);

        // Ability System
        EditorGUILayout.BeginHorizontal();
        {
            // Class Fields
            EditorGUILayout.BeginVertical();
            {
                abilitySystem.NextVisible(true);
                toggles[i] = EditorGUILayout.Foldout(toggles[i], new GUIContent(Enum.GetNames(typeof(PlayerAttacks.Attacks))[i]));
                
                if (toggles[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(abilitySystem);
                    Type abilitySystemType = Instance.attackSystems[i].GetType();

                    // magic
                    if (abilitySystemType == typeof(MagicAbilitySystem))
                    {
                        DrawMagicSystem(Instance.attackSystems[i]);
                    }
                    EditorGUI.indentLevel--;       
                }
                         
            }
            EditorGUILayout.EndVertical();

            // Type and New
            EditorGUILayout.BeginHorizontal(GUILayout.Width(50));
            {
                GUILayout.Label(Instance.attackSystems[i].GetType().ToString().Substring(0, Instance.attackSystems[i].GetType().ToString().Length-6));
                // create new ability
                if (GUILayout.Button(new GUIContent("New", Enum.GetNames(typeof(PlayerAttacks.Attacks))[i]), GUILayout.Width(35)))
                {
                    newAttack = i;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();

        // New class
        if (newAttack == i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // base
                if (GUILayout.Button("Base"))
                {
                    ((AttackSystems)target).attackSystems[i] = new AbilitySystem();
                    EditorUtility.SetDirty(target);
                    newAttack = -1;
                }

                // magic
                if (GUILayout.Button("Magic"))
                {
                    ((AttackSystems)target).attackSystems[i] = new MagicAbilitySystem();
                    EditorUtility.SetDirty(target);
                    newAttack = -1;
                }

                // cancel
                if (GUILayout.Button("Cancel"))
                {
                    newAttack = -1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    } // end DisplayAttackSystem


    private void DrawMagicSystem(AbilitySystem system)
    {
        EditorGUILayout.LabelField("hell");
    } // end DrawMagicSystem

} // end AttackSystems_Editor class