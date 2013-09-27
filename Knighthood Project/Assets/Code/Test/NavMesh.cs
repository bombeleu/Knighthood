// Steve Yeager
// 9.16.2013

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Navigation mesh for characters.
/// </summary>
[ExecuteInEditMode]
public class NavMesh : Singleton<NavMesh>
{
    public GameObject Node_Prefab;
    public float yBuffer = 0.5f;
    public float nodeRadius = 0.5f;
    public List<Node>[][] worldMatrix;
    public int gridSpace;
    public int gridHeight;
    public int gridWidth;

    public bool drawNodeConnections;
    public bool drawWorldMatrix;


    private void Awake()
    {
        BakeWorldMatrix(gridSpace, gridHeight, gridWidth);
    }


    public void CreateBaseNodes(float horDist)
    {
        DateTime start = DateTime.Now;

        GameObject[] terrainPieces = FindGameObjectsWithLayer("Terrain");
        List<Node> nodeList = new List<Node>();
        Node node;

        foreach (var terrain in terrainPieces)
        {
            float yPos = terrain.collider.bounds.center.y + terrain.collider.bounds.extents.y + yBuffer;
            float startXPos = terrain.collider.bounds.center.x - terrain.collider.bounds.extents.x;
            
            // left edge
            node = CreateNode(new Vector3(startXPos, yPos, 0f), true);
            node.position = node.transform.position + Vector3.down*yBuffer;
            // right edge
            node = CreateNode(new Vector3(terrain.collider.bounds.center.x + terrain.collider.bounds.extents.x, yPos, 0f), true);
            node.position = node.transform.position + Vector3.down * yBuffer;

            int num = Mathf.FloorToInt(terrain.collider.bounds.size.x / horDist);
            float aveHorDist = terrain.collider.bounds.size.x / num;

            for (int i = 1; i < num; i++)
            {
                Vector3 nodePosition = new Vector3(startXPos + i * aveHorDist, yPos, 0f);
                node = CreateNode(nodePosition);
                if (node)
                {
                    nodeList.Add(node);
                    node.position = node.transform.position + Vector3.down * yBuffer;
                }
            }
        }

        Debug.Log("Done Creating: " + (DateTime.Now - start));
    }


    public void Bake(float jumpHeight, float dropHorDist)
    {
        DateTime start = DateTime.Now;

        Transform[] allNodes = GetComponentsInChildren<Transform>();

        foreach (var node in allNodes)
        {
            try
            {
                node.GetComponent<Node>().Bake(allNodes, jumpHeight, dropHorDist);
            }
            catch (Exception)
            {
            }
        }
        
        Debug.Log("Done Baking: " + (DateTime.Now - start));
    }


    private GameObject[] FindGameObjectsWithLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        GameObject[] allGameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        
        List<GameObject> gameObjectsInLayer = allGameObjects.Where(go => go.layer == layer).ToList();

        if (gameObjectsInLayer.Count == 0) return null;
        return gameObjectsInLayer.ToArray();
    }


    private Node CreateNode(Vector3 position, bool edge = false)
    {
        if (Physics.OverlapSphere(position, nodeRadius).Length == 0)
        {
            GameObject node = (GameObject)Instantiate(Node_Prefab, position, Quaternion.identity);
            node.transform.parent = transform;
            if (edge)
            {
                node.GetComponent<Node>().edge = true;
            }
            return node.GetComponent<Node>();
        }

        return null;
    }


    public void BakeWorldMatrix(int gridSpace, int gridHeight, int gridWidth)
    {
        DateTime start = DateTime.Now;

        this.gridSpace = gridSpace;
        this.gridHeight = gridHeight;
        this.gridWidth = gridWidth;

        worldMatrix = new List<Node>[gridHeight][];
        for (int i = 0; i < gridHeight; i++)
        {
            worldMatrix[i] = new List<Node>[gridWidth];
            for (int j = 0; j < gridWidth; j++)
            {
                worldMatrix[i][j] = new List<Node>();
            }
        }

        Transform[] allNodes = GetComponentsInChildren<Transform>();

        for (int i = 1; i < allNodes.Length; i++)
        {
            int x = Mathf.FloorToInt(allNodes[i].transform.position.x/gridSpace);
            int y = gridHeight - 1 - Mathf.FloorToInt(allNodes[i].transform.position.y / gridSpace);
            //int y = Mathf.FloorToInt(allNodes[i].transform.position.y / gridSpace);
            try
            {
                worldMatrix[y][x].Add(allNodes[i].GetComponent<Node>());
            }
            catch (Exception)
            {
                Debug.Log(gridHeight);
                Debug.Log(String.Format("[{0}][{1}]", y, x));
                throw;
            }
            
        }

        Debug.Log("Done Baking World Matrix: " + (DateTime.Now - start));
        //for (int i = 0; i < gridHeight; i++)
        //{
        //    for (int j = 0; j < gridWidth; j++)
        //    {
        //        Debug.Log(worldMatrix[i][j].Count);
        //    }
        //}
    }


    [Obsolete]
    public void Simplify()
    {
        DateTime start = DateTime.Now;
        int deleted = 0;

        Node[] allNodes = GetComponentsInChildren<Node>();

        foreach (var node in allNodes)
        {
            if (!node.edge && node.neighbors.Length <= 2)
            {
                DestroyImmediate(node.gameObject, true);
            }
        }

        Debug.Log("Done Simplify: " + (DateTime.Now - start) + " Deleted: " + deleted);
    }


    public Node ClosestNode(Vector3 position)
    {
        List<Node> nodes = new List<Node>();

        int x = Mathf.FloorToInt(position.x / gridSpace);
        x = Mathf.Clamp(x, 0, gridWidth - 1);
        int y = gridHeight - 1 - Mathf.FloorToInt(position.y / gridSpace);
        y = Mathf.Clamp(y, 0, gridHeight - 1);

        nodes = worldMatrix[y][x];

        // check above
        if (nodes.Count == 0 && y > 0)
        {
            nodes = worldMatrix[y - 1][x];
        }

        float closestDist = gridSpace+1;
        Node closest = null;
        foreach (var node in nodes)
        {
            if (Vector3.Distance(position, node.transform.position) <= closestDist)
            {
                closestDist = Vector3.Distance(position, node.transform.position);
                closest = node;
            }
        }

        return closest;
    }


    private void Update()
    {
        if (drawWorldMatrix)
        {
            DrawWorldMatrix();
        }
    }


    private void DrawWorldMatrix()
    {
        // hor
        for (int i = 0; i <= gridHeight; i++)
        {
            Debug.DrawLine(new Vector3(0, gridSpace * i), new Vector3(gridSpace * gridWidth, gridSpace * i), Color.yellow);
        }

        // ver
        for (int i = 0; i <= gridWidth; i++)
        {
            Debug.DrawLine(new Vector3(gridSpace * i, 0), new Vector3(gridSpace * i, gridSpace * gridHeight), Color.yellow);
        }
    }


    public void ToggleNodeConnections()
    {
        drawNodeConnections = !drawNodeConnections;
        var nodes = GetComponentsInChildren<Node>();
        foreach (var node in nodes)
        {
            node.drawConnections = drawNodeConnections;
        }


        SceneView.RepaintAll();
    }
}