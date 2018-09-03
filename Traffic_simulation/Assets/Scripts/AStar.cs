using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

  [Serializable]
  public struct AStarRoad
  {
    public GameObject element;
    public float distanceFromEnd;

    public AStarRoad(GameObject e, float d)
    {
      element = e;
      distanceFromEnd = d;
    }
  }
  public List<AStarRoad> roadPath;

  public GameObject endObj;
  public GameObject startObj;

  // Use this for initialization
  void Start () {
    roadPath = new List<AStarRoad>();
    foreach (var item in ElementTable.roadToPosition)
    {
      roadPath.Add(new AStarRoad(item.element,float.PositiveInfinity));
    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  /*public List<GameObject> GetShortestPathAstar()
  {
    for (int i = 0; i < roadPath.Count; i++)
    {
      //this is disgusting
      AStarRoad temp = roadPath[i];
      temp.distanceFromEnd = CalculateDistance(roadPath[i].element, endObj);
      roadPath[i] = temp;
    }
      
    AstarSearch();
    var shortestPath = new List<GameObject>();
    shortestPath.Add(endObj);
    BuildShortestPath(shortestPath, endObj);
    shortestPath.Reverse();
    return shortestPath;
  }

  private float CalculateDistance(GameObject element, GameObject endObj)
  {
    return Vector3.Distance(element.transform.position, endObj.transform.position);
  }

  private void AstarSearch()
  {
    Start.MinCostToStart = 0;
    var prioQueue = new List<Node>();
    prioQueue.Add(Start);
    do
    {
      prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
      var node = prioQueue.First();
      prioQueue.Remove(node);
      NodeVisits++;
      foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
      {
        var childNode = cnn.ConnectedNode;
        if (childNode.Visited)
          continue;
        if (childNode.MinCostToStart == null ||
            node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
        {
          childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
          childNode.NearestToStart = node;
          if (!prioQueue.Contains(childNode))
            prioQueue.Add(childNode);
        }
      }
      node.Visited = true;
      if (node == End)
        return;
    } while (prioQueue.Any());
  }

  private void BuildShortestPath(List<Node> list, Node node)
  {
    if (node.NearestToStart == null)
      return;
    list.Add(node.NearestToStart);
    BuildShortestPath(list, node.NearestToStart);
  }
  */
}
