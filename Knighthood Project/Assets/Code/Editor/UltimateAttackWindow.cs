// Steve Yeager
// 10.19.2013

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class UltimateAttackWindow : EditorWindow
{
    #region Private Fields

    private UltimateAttackManager myManager;
    private Vector2 scrollPosition;
    private bool attacksToggle;
    private int playerValue;
    private int[] attacks;
    private bool[] attackToggle = new bool[15];

    #endregion


    #region EditorWindow Overrides

    [MenuItem("Tools/Ultimate Attack Manager %U")]
    private static void Init()
    {
        GetWindow(typeof(UltimateAttackWindow), false, "Ultimates");
    }


    [MenuItem("Tools/Ultimate Attack Manager %U", true)]
    private static bool Validator()
    {
        return true;
    }


    private void OnSelectionChange()
    {
        myManager = null;
        title = "Ultimates";
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<UltimateAttackManager>() != null)
        {
            myManager = Selection.activeGameObject.GetComponent<UltimateAttackManager>();
            title = myManager.name + " Ults";
            playerValue = myManager.playerValue;
            attacks = UltimateAttackManager.participantAttacks[playerValue];
        }
        Repaint();
    }


    void OnFocus()
    {
        Debug.Log("here");
    }


    private void OnGUI()
    {
        if (myManager == null)
        {
            GUILayout.Label("Need to select a Player");
            return;
        }

        //EditorGUIUtility.LookLikeInspector();

        // attacks
        //attacksToggle = EditorGUILayout.Foldout(attacksToggle, new GUIContent("Attacks"));
        //if (attacksToggle)
        //{
        //    EditorGUI.indentLevel++;
            
        //    // main controls

        //    foreach (int attackValue in attacks)
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        {
        //            if (GUILayout.Button(attackToggle[attackValue] ? "-" : "+", GUILayout.Width(30f)))
        //            {
        //                attackToggle[attackValue] = !attackToggle[attackValue];
        //            }

        //            myManager.attackNames[attackValue] = EditorGUILayout.TextField(myManager.attackNames[attackValue], GUILayout.MaxWidth(100f));
        //            string participantString = "";
        //            UltimateAttackManager.Participants[] participants = UltimateAttackManager.attackParcipants[attackValue];
        //            for (int i = 0; i < participants.Length - 1; i++)
        //            {
        //                participantString += participants[i].ToString() + ", ";
        //            }
        //            participantString += participants[participants.Length-1].ToString();
        //            EditorGUILayout.LabelField(" - " + participantString);
        //        }
        //        EditorGUILayout.EndHorizontal();

        //        // attackValue
        //        if (attackToggle[attackValue])
        //        {
        //            DrawAttack(attackValue);
        //        }
        //    }

        //    EditorGUI.indentLevel--;
        //}

        //scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        //{

        //}
        //EditorGUILayout.EndScrollView();
    }

    #endregion

    #region Private Methods

    private void DrawAttack(int attack)
    {
        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("attackStat: " + attack);

        EditorGUI.indentLevel--;
    }

    #endregion
}