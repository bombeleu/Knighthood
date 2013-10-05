// Steve Yeager
// 10.4.2013

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    #region Private Fields

    private SerializedObject SO;
    private Health myHealth;
    private string[] statusEffects;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        SO = new SerializedObject(target);
        myHealth = (Health)target;

        statusEffects = Enum.GetNames(typeof (HitInfo.Effects));
    }


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        SO.Update();

        // log
        EditorGUILayout.PropertyField(SO.FindProperty("log"), new GUIContent("Log"));

        // max health
        int maxHealth = EditorGUILayout.IntField("Max Health", myHealth.maxHealth);
        if (maxHealth < 1) maxHealth = 1;
        myHealth.maxHealth = maxHealth;

        // current health
        int currentHealth = EditorGUILayout.IntField("Current Health", myHealth.currentHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        myHealth.currentHealth = currentHealth;

        // display health
        GUILayout.Space(4f);
        Rect rect = EditorGUILayout.BeginVertical();
        {
            rect.x += 16f;
            rect.width -= 32f;
            EditorGUI.ProgressBar(rect, (float)currentHealth / maxHealth, "Health");
            GUILayout.Space(16f);
        }
        EditorGUILayout.EndVertical();

        // status effectivenesses
        GUILayout.Space(4f);
        EditorGUILayout.LabelField("Effects");
        EditorGUI.indentLevel++;
        for (int i = 0; i < myHealth.statusEffectivenesses.Length; i++)
        {
            Rect rectColor = EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.DrawColorSwatch(rectColor, Color.Lerp(Color.green, Color.red, myHealth.statusEffectivenesses[i]/2));
                EditorGUILayout.LabelField(statusEffects[i], GUILayout.MaxWidth(100f));
                SO.FindProperty("statusEffectivenesses").GetArrayElementAtIndex(i).floatValue =
                    EditorGUILayout.Slider(
                        SO.FindProperty("statusEffectivenesses").GetArrayElementAtIndex(i).floatValue, 0f, 2f);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        
        SO.ApplyModifiedProperties();
    }

    #endregion
}