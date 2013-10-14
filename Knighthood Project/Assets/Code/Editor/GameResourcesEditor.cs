// Steve Yeager
// 10.8.2013

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameResources))]
public class GameResourcesEditor : Editor
{
    #region Private Fields

    private SerializedObject SO;
    private GameResources myGR;
    private bool enemyPrefabsToggle;

    #endregion


    #region Editor Overrides

    private void OnEnable()
    {
        SO = new SerializedObject(target);
        myGR = (GameResources)target;
    }


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        SO.Update();

        EditorGUILayout.PropertyField(SO.FindProperty("log"));
        EditorGUILayout.PropertyField(SO.FindProperty("Player_Prefabs"), true);

        EditorGUI.indentLevel--;
        enemyPrefabsToggle = EditorGUILayout.Foldout(enemyPrefabsToggle, "Enemy Prefabs");
        EditorGUI.indentLevel+=2;
        if (enemyPrefabsToggle)
        {
            for (int i = 0; i < myGR.EnemyPrefabTypes.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    // enemy
                    EditorGUILayout.PropertyField(SO.FindProperty("EnemyPrefabTypes").GetArrayElementAtIndex(i),
                        new GUIContent(""), GUILayout.MinWidth(100f));
                    // prefab
                    EditorGUILayout.PropertyField(SO.FindProperty("Enemy_Prefabs").GetArrayElementAtIndex(i),
                        new GUIContent(""), GUILayout.MinWidth(100f));
                    // delete
                    if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
                    {
                        SO.FindProperty("EnemyPrefabTypes").DeleteArrayElementAtIndex(i);
                        SO.FindProperty("Enemy_Prefabs").DeleteArrayElementAtIndex(i);
                        SO.ApplyModifiedProperties();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            // add new
            if (GUILayout.Button("Add New"))
            {
                SO.FindProperty("EnemyPrefabTypes").InsertArrayElementAtIndex(myGR.EnemyPrefabTypes.Length);
                SO.FindProperty("Enemy_Prefabs").InsertArrayElementAtIndex(myGR.Enemy_Prefabs.Length);
                SO.ApplyModifiedProperties();
            }
        }
        EditorGUI.indentLevel -= 2;

        SO.ApplyModifiedProperties();
    }

    #endregion
}