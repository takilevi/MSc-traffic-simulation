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

  public List<GameObject> calculatedRoute;
  public GameObject from;
  public GameObject to;

  void Awake () {
    allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadModel");
    allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
    allRoad = new List<GameObject>();
    allRoad.AddRange(allRoadGameObject);
    allRoad.AddRange(allCrossRoadGameObject);

    StartDiscover(discoverStartObj);

    foreach (var item in allRoadGameObject)
    {
      RoadElementModel element = item.GetComponent<RoadElementModel>();
      if (element.NextElement == null || element.PreviousElement == null)
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

    calculatedRoute = new List<GameObject>();
    FindLongestPath(from, to);
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

  void FindLongestPath( GameObject from, GameObject to)
  {
    calculatedRoute.Add(from);

    bool straightRoad = true;
    List<GameObject> exits;
    while (straightRoad)
    {
      GameObject nextElement = calculatedRoute[calculatedRoute.Count - 1];

      if (GameObject.ReferenceEquals(nextElement, to))
      {
        return;
      }
      Component nextComp = nextElement.GetComponent<CrossRoadModel>();
      if(nextComp != null)
      {
        exits = nextComp.GetComponentInParent<CrossRoadMeta>().GetOptions(nextElement);
        straightRoad = false;
      }
    }

    //ha megvan az elem akkor már kiléptünk
    //ha megvan a legközelebbi kereszteződés akkor ide jutottunk, a lehetséges kimenetelek a exits változóban
  }
}
