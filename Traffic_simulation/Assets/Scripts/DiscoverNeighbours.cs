using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DiscoverNeighbours : MonoBehaviour {
  public Dictionary<GameObject,GameObject> carsToDestinations;
  public GameObject discoverStartObj;
  public bool manualTriggerRediscover = false;

  public GameObject[] allRoadGameObject;
  public GameObject[] allCrossRoadGameObject;
  List<GameObject> allRoad;

  void Awake () {
    allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadModel");
    allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
    allRoad = new List<GameObject>();
    allRoad.AddRange(allRoadGameObject);
    allRoad.AddRange(allCrossRoadGameObject);

    /*
     * This snippet nulls all the references, therefore forces the rediscover!!
     * 
    foreach (var item in allRoadGameObject)
    {
      RoadElementModel element = item.GetComponent<RoadElementModel>();
      element.NextElement = null;
      element.PreviousElement = null;
    }
    foreach (var item in allCrossRoadGameObject)
    {
      CrossRoadModel element = item.GetComponent<CrossRoadModel>();
      element.ClosestRoad = null;
    }
      *
      *
      */

    foreach (var item in allRoadGameObject)
    {
      RoadElementModel element = item.GetComponent<RoadElementModel>();
      if(element.NextElement == null || element.PreviousElement == null)
      {
        Debug.Log("VAN MÉG NULLOS!! " + item.name + "...." + item.transform.parent.name);
      }
    }
    foreach (var item in allCrossRoadGameObject)
    {
      CrossRoadModel element = item.GetComponent<CrossRoadModel>();
      if (element.ClosestRoad == null)
      {
        Debug.Log("VAN MÉG NULLOS!! " + item.name + "...." + item.transform.parent.name);
      }
    }

    StartDiscover(discoverStartObj);

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void StartDiscover(GameObject startObj)
  {
    foreach (Transform child in startObj.transform)
    {
      child.gameObject.GetComponent<RoadElementModel>().FindMyNeighbours();
    }
  }
}
