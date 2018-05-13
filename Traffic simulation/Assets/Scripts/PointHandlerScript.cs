using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PointHandlerScript : MonoBehaviour
{
  GameObject[] allRoadGameObject;
  GameObject[] allCrossRoadGameObject;

  public GameObject car;
  public GameObject popcorn;
  public GameObject closestObject;
  public Vector3 forwardVector;
  public GameObject endObject;

  // Use this for initialization
  void Awake()
  {
    //PathPointRecreateAndNull();

    PathCalculator();
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void PathPointRecreateAndNull()
  {
    var foundObjects = FindObjectsOfType<TransformModifier>();
    foreach (var item in foundObjects)
    {
      var components = item.GetComponents(typeof(TransformModifier));
      foreach (var comp in components)
      {
        //DestroyImmediate instead of Destroy!!!!
        DestroyImmediate(comp);
      }
    }

    allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadElement");
    allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");

    foreach (var item in allRoadGameObject)
    {
      item.AddComponent<RoadElementModel>().thisColor = Color.blue;
    }
    foreach (var item in allCrossRoadGameObject)
    {
      item.AddComponent<CrossRoadModel>().thisColor = Color.cyan;
    }
  }

  public void PathCalculator()
  {
    car = GameObject.Find("MUSTANG");
    popcorn = GameObject.Find("POPCORN");
    forwardVector = car.transform.forward;
    closestObject = GetClosestElement(car.transform.position);
    endObject = GetClosestElement(popcorn.transform.position);

    Debug.Log("mustang start object neve: " + closestObject.name + "  pozi: " + closestObject.GetComponent<RoadElementModel>().Position);
    Debug.Log("dist: " + Vector3.Distance(car.transform.position, closestObject.GetComponent<RoadElementModel>().Position));
    Debug.Log("popcorn end object neve: " + endObject.name);
  }

  public GameObject GetClosestElement(Vector3 carPos)
  {
    GameObject theChoosenOne = null;
    foreach (var item in allRoadGameObject)
    {
      if (theChoosenOne == null)
      {
        theChoosenOne = item;
      }
      float dist = Vector3.Distance(carPos, theChoosenOne.GetComponent<RoadElementModel>().Position);
      float newDist = Vector3.Distance(carPos, item.GetComponent<RoadElementModel>().Position);

      if (newDist < dist)
      {
        theChoosenOne = item;
      }

    }
    return theChoosenOne;
  }

}
