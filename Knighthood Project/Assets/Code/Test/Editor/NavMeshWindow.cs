// Steve Yeager
// 9.16.2013

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Window for creating, editing, and deleting the level's NavMesh.
/// </summary>
public class NavMeshWindow : EditorWindow
{
    private float targetNodeSep = 3f;
    private float jumpHeight = 10f;
    private float dropHorDist = 4f;
    private int gridSpace = 5;
    private int gridHeight = 9;
    private int gridWidth = 21;

    private bool baking;


    #region EditorWindow Overrides

    [MenuItem("Tools/Nav Mesh")]
    private static void Init()
    {
        NavMeshWindow window = (NavMeshWindow)GetWindow(typeof(NavMeshWindow));
        window.title = "NavMesh";
    }


    private void OnGUI()
    {
        if (GameObject.Find("NavMesh") == null)
        {
            if (GUILayout.Button("Create NavMesh"))
            {
                CreateNavMesh();
            }
            return;
        }

        targetNodeSep = EditorGUILayout.FloatField("Target Node Separation", targetNodeSep);
        jumpHeight = EditorGUILayout.FloatField("Jump Height", jumpHeight);
        dropHorDist = EditorGUILayout.FloatField("Drop Hor Distance", dropHorDist);

        if (GUILayout.Button((NavMesh.Instance.drawNodeConnections ? "Hide" : "Show") + " Node Connections"))
        {
            NavMesh.Instance.ToggleNodeConnections();
        }

        if (GUILayout.Button("Create Base Nodes"))
        {
            NavMesh.Instance.CreateBaseNodes(targetNodeSep);
        }

        if (GUILayout.Button("Bake"))
        {
            NavMesh.Instance.Bake(jumpHeight, dropHorDist);            
        }

        if (GUILayout.Button("Simplify"))
        {
            NavMesh.Instance.Simplify();
            NavMesh.Instance.Bake(jumpHeight, dropHorDist);
        }

        if (GUILayout.Button("Clear"))
        {
            while (NavMesh.Instance.transform.childCount > 0)
            {
                DestroyImmediate(NavMesh.Instance.transform.GetChild(0).gameObject, true);
            }
        }

        gridSpace = EditorGUILayout.IntField("Matrix Cell Space", gridSpace);
        gridHeight = EditorGUILayout.IntField("Matrix Height", gridHeight);
        gridWidth = EditorGUILayout.IntField("Matrix Width", gridWidth);

        NavMesh.Instance.drawWorldMatrix = EditorGUILayout.Toggle("Draw World Matrix", NavMesh.Instance.drawWorldMatrix);

        if (GUILayout.Button("Bake World Matrix"))
        {
            NavMesh.Instance.BakeWorldMatrix(gridSpace, gridHeight, gridWidth);
            EditorUtility.SetDirty(NavMesh.Instance);
        }
    }

    #endregion

    #region Private Methods

    private static void CreateNavMesh()
    {
        NavMesh.Instance.transform.parent = GameObject.Find("World").transform;
    }

    #endregion
}