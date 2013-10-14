// Steve Yeager
// 9.16.2013

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls character for navigation.
/// </summary>
public class NavAgent : BaseMono
{
    #region Cached Fields

    private Transform myTransform;

    #endregion

    #region Private Fields

    private List<Node> path = new List<Node>();
    private PriorityQueue<float, Node> openList = new PriorityQueue<float, Node>();
    private Dictionary<Node, bool> closedDict = new Dictionary<Node, bool>();
    private Dictionary<Node, Node> parentDict = new Dictionary<Node, Node>();
    private Dictionary<Node, float> costDict = new Dictionary<Node, float>();
    private Node currentNode;
    private Job calculatePath = null;

    #endregion

    #region Properties

    public int pathLength
    {
        get
        {
            return path.Count;
        }
    }

    #endregion

    #region Public Fields

    public bool drawPath;
    // make into a method
    public float allowedRadius = 0.5f;
    // make into a method
    public float stepHeight = 0.5f;
    /// <summary>Time between calculating new paths.</summary>
    public float navBuffer = 0.5f;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // cache
        myTransform = transform;
    }


    private void Update()
    {
        if (drawPath)
        {
            DrawPath();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start navigating towards next node.
    /// </summary>
    /// <returns>False, if there are no more nodes.</returns>
    public bool Continue()
    {
        if (path.Count == 0) return false;

        currentNode = path[0];
        path.RemoveAt(0);
        return path.Count > 1;
    }


    /// <summary>
    /// Start navigating towards next node.
    /// </summary>
    /// <param name="nodePosition">Position of the next node.</param>
    /// <returns>False, if there are no more nodes.</returns>
    public bool Continue(out Vector3 nodePosition)
    {
        if (path.Count == 0)
        {
            nodePosition = currentNode.position;
            return false;
        }

        currentNode = path[0];
        path.RemoveAt(0);

        return GetNextNodePosition(out nodePosition);
    }


    /// <summary>
    /// Gets the position of the next node in the path.
    /// </summary>
    /// <param name="nodePosition">Position to be set for the next node.</param>
    /// <returns>False, if there are no more nodes.</returns>
    public bool GetNextNodePosition(out Vector3 nodePosition)
    {
        if (path.Count > 0)
        {
            nodePosition = path[0].position;
            return true;
        }
        else
        {
            nodePosition = currentNode.position;
            return false;
        }
    }


    /// <summary>
    /// Start path calculations.
    /// </summary>
    /// <param name="Target">Transform of the target.</param>
    public void StartNav(Transform Target)
    {
        Log("StartNav", Debugger.LogTypes.Navigation);

        if (calculatePath == null)
        {
            calculatePath = new Job(CalculatePath(Target));
        }
        else
        {
            calculatePath.UnPause();
        }
    }


    /// <summary>
    /// Stop path calculations.
    /// </summary>
    public void EndNav()
    {
        Log("EndNav", Debugger.LogTypes.Navigation);

        calculatePath.Pause();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Find a path to target.
    /// </summary>
    /// <param name="targetPosition">Position of the target.</param>
    private void FindPath(Vector3 targetPosition)
    {
        DateTime start = DateTime.Now;

        // ending node
        Node endNode = NavMesh.Instance.ClosestNode(targetPosition);
        if (path.Count > 0 && endNode == path[path.Count - 1])
        {
            return;
        }

        // reset
        ClearSearchData();

        // starting node
        //Node startNode = path.Count == 0 ? NavMesh.Instance.ClosestNode(myTransform.position) : path[0];
        Node startNode = NavMesh.Instance.ClosestNode(myTransform.position + Mathf.Sign((targetPosition - myTransform.position).x) * Vector3.right);
        if (endNode == null || startNode == null) return;
        openList.Enqueue(0f, startNode);
        closedDict.Add(startNode, false);
        parentDict.Add(startNode, null);
        costDict.Add(startNode, 0f);

        // main loop
        while (openList.Count > 0)
        {
            Node node = openList.DequeueValue();

            // goal?
            if (node == endNode)
            {
                path.Clear();
                while (node != startNode)
                {
                    path.Add(node);
                    node = parentDict[node];
                }

                if (path.Count > 0)
                {
                    path.Reverse();
                    currentNode = path[0];
                }
                Log("Path time: " + (DateTime.Now - start), Debugger.LogTypes.Navigation);
                return;
            }

            // process current node
            closedDict[node] = true;

            // look at all of current node's neighbors
            for (int i = 0; i < node.neighbors.Length; i++) // maybe cache size
            {
                // cache neighbor
                Node neighbor = node.neighbors[i];

                // get cost of whole path to neighbor
                float cost = costDict[node] + node.distances[i];

                // see if already open or closed
                if (!closedDict.ContainsKey(neighbor))
                {
                    // add to open
                    openList.Enqueue(cost + Vector3.Distance(neighbor.position, endNode.position), neighbor);
                    closedDict.Add(neighbor, false);
                    parentDict.Add(neighbor, node);
                    costDict.Add(neighbor, cost);
                }
            }
        }
    }


    /// <summary>
    /// Clear everything except path.
    /// </summary>
    private void ClearSearchData()
    {
        openList.Clear();
        closedDict.Clear();
        parentDict.Clear();
        costDict.Clear();
    }


    /// <summary>
    /// Debug. Draw path.
    /// </summary>
    private void DrawPath()
    {
        if (path.Count == 0) return;

        Debug.DrawLine(myTransform.position, path[0].transform.position, Color.blue);        
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].transform.position, path[i + 1].transform.position, Color.blue);
        }
    }


    /// <summary>
    /// Continuously recalculate path.
    /// </summary>
    /// <param name="Target">Target agent is chasing.</param>
    private IEnumerator CalculatePath(Transform Target)
    {
        while (true)
        {
            FindPath(Target.position);
            yield return WaitForTime(navBuffer);
        }
    }

    #endregion
}