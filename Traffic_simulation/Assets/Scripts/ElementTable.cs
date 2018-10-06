using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class ElementTable : MonoBehaviour
{

  [Serializable]
  public struct RoadAndPosition
  {
    public GameObject element;
    public Vector3 position;

    public RoadAndPosition(GameObject e, Vector3 p)
    {
      element = e;
      position = p;
    }
  }
  public static List<RoadAndPosition> roadToPosition;
  public static List<GameObject> allRoadElements;

  // Use this for initialization
  void Awake()
  {

    if (roadToPosition == null || roadToPosition.Count == 0)
    {
      Setup();
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  public static void Setup()
  {
    roadToPosition = new List<RoadAndPosition>();
    allRoadElements = new List<GameObject>();

    List<GameObject> allRoad = new List<GameObject>();

    allRoadElements.AddRange(GameObject.FindGameObjectsWithTag("RoadModel"));
    allRoad.AddRange(allRoadElements);

    allRoad.AddRange(GameObject.FindGameObjectsWithTag("CrossRoad"));

    foreach (var element in allRoad)
    {
      roadToPosition.Add(new RoadAndPosition(element, element.transform.position));
    }

    GameObject discoverGameObject = allRoadElements[Random.Range(0, allRoadElements.Count)].transform.parent.gameObject;
    StartDiscover(discoverGameObject);
  }

  public static GameObject MyClosestNeighbour(GameObject whom, Vector3 forward)
  {
    Vector3 whomWithForward = whom.transform.position + forward;
    //Debug.Log("my posi a forward*5-el együtt: " + whomWithForward);
    GameObject closest = null;

    foreach (var item in roadToPosition)
    {
      if (closest == null)
      {
        closest = item.element;
      }

      if (Vector3.Distance(whomWithForward, item.position) < 1)
      {
        //Debug.Log(Vector3.Distance(whomWithForward, item.position));
        closest = item.element;
        return closest;
      }

      if (Vector3.Distance(whomWithForward, item.position)
        < Vector3.Distance(whomWithForward, closest.transform.position))
      {
        closest = item.element;
      }
    }

    return closest;
  }

  public static GameObject GetClosestElement(Vector3 targetPos)
  {
    GameObject closest = null;
    if (roadToPosition == null || roadToPosition.Count == 0) { Setup(); }

    foreach (var item in roadToPosition)
    {
      if (closest == null)
      {
        closest = item.element;
      }

      if (Vector3.Distance(targetPos, item.position) < 1)
      {
        //Debug.Log(Vector3.Distance(whomWithForward, item.position));
        closest = item.element;
        return closest;
      }

      if (Vector3.Distance(targetPos, item.position)
        < Vector3.Distance(targetPos, closest.transform.position))
      {
        closest = item.element;
      }
    }

    return closest;
  }

  static void StartDiscover(GameObject startObj)
  {
    foreach (Transform child in startObj.transform)
    {
      child.gameObject.GetComponent<RoadElementModel>().FindMyNeighbours();
    }
  }

  public static GameObject GetRandomRoadElement()
  {
    if (allRoadElements == null || allRoadElements.Count == 0 
      || roadToPosition == null || roadToPosition.Count == 0) { Setup(); }

    return allRoadElements[Random.Range(0, allRoadElements.Count)];
  }
}
