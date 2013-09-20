// Steve Yeager
// 9.16.2013

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls character for navigation.
/// </summary>
public class NavAgent : BaseMono
{
    private List<NavMeshNode> path = new List<NavMeshNode>();
    private PriorityQueue<float, NavMeshNode> openList = new PriorityQueue<float, NavMeshNode>();
    private Dictionary<NavMeshNode, bool> closedDict = new Dictionary<NavMeshNode, bool>();
    private Dictionary<NavMeshNode, NavMeshNode> parentDict = new Dictionary<NavMeshNode, NavMeshNode>();
    private Dictionary<NavMeshNode, float> costDict = new Dictionary<NavMeshNode, float>();

    public Transform nextNode
    {
        get
        {
            return path.Count > 0 ? path[0].transform : null;
        }
    }

    #region MonoBehaviour Overrides

    private void Update()
    {
        //FindPath(playerTransform.position);

        DrawPath();
    }

    #endregion

    #region Public Methods

    public void FindPath(Vector3 targetPosition)
    {
        DateTime start = DateTime.Now;

        // reset
        ClearSearchData();

        // ending node
        NavMeshNode endNode = NavMesh.Instance.ClosestNode(targetPosition);

        // starting node
        NavMeshNode startNode = path.Count == 0 ? NavMesh.Instance.ClosestNode(transform.position) : path[0];
        if (endNode == null || startNode == null) return;
        openList.Enqueue(0f, startNode);
        closedDict.Add(startNode, false);
        parentDict.Add(startNode, null);
        costDict.Add(startNode, 0f);

        // main loop
        while (openList.Count > 0)
        {
            NavMeshNode currentNode = openList.DequeueValue();

            // goal?
            if (currentNode == endNode)
            {
                path.Clear();
                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = parentDict[currentNode];
                }
                path.Reverse();
                Debug.Log("Path time: " + (DateTime.Now - start));
                //Debug.Log("Path: " + path.Count);
                return;
            }

            // process current node
            closedDict[currentNode] = true;

            // look at all of current node's neighbors
            for (int i = 0; i < currentNode.neighbors.Length; i++) // maybe cache size
            {
                // cache neighbor
                NavMeshNode neighbor = currentNode.neighbors[i];

                // get cost of whole path to neighbor
                float cost = costDict[currentNode] + currentNode.distances[i];

                // see if already open or closed
                if (!closedDict.ContainsKey(neighbor))
                {
                    // add to open
                    openList.Enqueue(cost + Vector3.Distance(neighbor.transform.position, endNode.transform.position), neighbor);
                    closedDict.Add(neighbor, false);
                    parentDict.Add(neighbor, currentNode);
                    costDict.Add(neighbor, cost);
                }
            }
        }
    }


    public Transform Continue()
    {
        path.RemoveAt(0);
        if (path.Count > 0)
        {
            return path[0].transform;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region Private Methods

    private void ClearSearchData()
    {
        openList.Clear();
        closedDict.Clear();
        parentDict.Clear();
        costDict.Clear();
    }


    private void DrawPath()
    {
        for (int i = 0; i < path.Count-1; i++)
        {
            Debug.DrawLine(path[i].transform.position, path[i+1].transform.position, Color.blue);
        }
    }

    #endregion
}