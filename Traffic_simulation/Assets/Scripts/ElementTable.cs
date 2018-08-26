using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ElementTable : MonoBehaviour {

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

	// Use this for initialization
	void Awake () {

    roadToPosition = new List<RoadAndPosition>();
    List<GameObject> allRoad = new List<GameObject>();

    allRoad.AddRange(GameObject.FindGameObjectsWithTag("RoadModel"));
    allRoad.AddRange(GameObject.FindGameObjectsWithTag("CrossRoad"));

    foreach (var element in allRoad)
    {
      roadToPosition.Add(new RoadAndPosition(element, element.transform.position));
    }
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public static GameObject MyClosestNeighbour(GameObject whom, Vector3 forward)
  {
    Vector3 whomWithForward = whom.transform.position + forward;
    //Debug.Log("my posi a forward*5-el együtt: " + whomWithForward);
    GameObject closest = null;

    foreach (var item in roadToPosition)
    {
      if(closest == null)
      {
        closest = item.element;
      }

      if(Vector3.Distance(whomWithForward, item.position) < 1)
      {
        //Debug.Log(Vector3.Distance(whomWithForward, item.position));
        closest = item.element;
        return closest;
      }

      if(Vector3.Distance(whomWithForward, item.position) 
        < Vector3.Distance(whomWithForward, closest.transform.position))
      {
        closest = item.element;
      }
    }

    return closest;
  }
}
