using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PointHandlerScript : MonoBehaviour
{
  GameObject[] allRoadGameObject;
  GameObject[] allCrossRoadGameObject;
  List<GameObject> allRoad;

  public GameObject car;
  public GameObject popcorn;
  public GameObject closestObject;
  public GameObject endObject;
  public Vector3 forwardVector;

  public List<GameObject> route;

  // Use this for initialization
  void Awake()
  {
    car.GetComponent<TweenHelper>().test = null;
    car.GetComponent<TweenHelper>().testV3 = null;
    allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadElement");
    allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
    allRoad = new List<GameObject>();
    allRoad.AddRange(allRoadGameObject);
    allRoad.AddRange(allCrossRoadGameObject);

    route = new List<GameObject>();

    PathPointRecreateAndNull();

    PathCalculator();

    if (car != null && popcorn != null && closestObject != null && endObject != null)
    {
      route.Add(closestObject);
      RouteCalculator();
    }

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void PathPointRecreateAndNull()
  {
    var foundObjects = FindObjectsOfType<CrossRoadModel>();
    foreach (var item in foundObjects)
    {
      var components = item.GetComponents(typeof(CrossRoadModel));
      foreach (var comp in components)
      {
        //DestroyImmediate instead of Destroy!!!!
        DestroyImmediate(comp);
      }
    }

    foreach (var item in allRoadGameObject)
    {
      var component = item.GetComponent<RoadElementModel>();
      GameObject next = component.NextElement;

      DestroyImmediate(component);
      item.AddComponent<RoadElementModel>().thisColor = Color.blue;

      if (next != null)
      {
        item.GetComponent<RoadElementModel>().NextElement = next;

      }

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

    Debug.Log("mustang start object neve: " + closestObject.name + "  pozi: " + closestObject.GetComponent<RoadElementModel>().Position +
      "  dist: " + Vector3.Distance(car.transform.position, closestObject.GetComponent<RoadElementModel>().Position) +
      "  forward: " + forwardVector);
    Debug.Log("popcorn end object neve: " + endObject.name);
  }

  public GameObject GetClosestElement(Vector3 carPos)
  {
    GameObject theChoosenOne = null;
    foreach (var item in allRoad)
    {
      if (theChoosenOne == null)
      {
        theChoosenOne = item;
      }
      float dist = Vector3.Distance(carPos, theChoosenOne.GetComponent<TransformModifier>().Position);
      float newDist = Vector3.Distance(carPos, item.GetComponent<TransformModifier>().Position);

      if (newDist < dist)
      {
        theChoosenOne = item;
      }

    }
    return theChoosenOne;
  }

  public void RouteCalculator()
  {

    GameObject nextOne = nextElement();
    while (nextOne.name != endObject.name)
    {
      route.Add(nextOne);
      if (nextOne.tag == "RoadElement")
      {
        nextOne = nextElement();
      }
			if (nextOne.tag == "CrossRoad")
			{
				Debug.Log("CrossRoad branch start at: " + System.DateTime.Now);
				CrossRoadMeta parentMeta = nextOne.GetComponentInParent<CrossRoadMeta>();

				//amit a függvény visszaad ahhoz még hozzá kell csapni a kereszteződés utáni első roadelementet!!!
				Debug.Log("before crossroad calculate call: " + System.DateTime.Now);
				/*
				 * 
				 * crossroad calculation
				 * 
				 */
				GameObject[] addables = { };
        Debug.Log("after crossroad calculate call: " + System.DateTime.Now);
        route.AddRange(addables);

        /*foreach (var item in route)
        {
          Debug.Log("route elements: " + item.name);
        }
        */

        Vector3 newForward = (route[route.Count - 1].GetComponent<TransformModifier>().Position
                - route[route.Count - 2].GetComponent<TransformModifier>().Position).normalized;

        GameObject theNext = GetClosestElement(route[route.Count - 1].GetComponent<TransformModifier>().Position + newForward * 5);
        Debug.Log("kereszteződés utáni elem: " + theNext.name + " ......   " + newForward);

        nextOne = theNext;
        Debug.Log("CrossRoad branch end at: " + System.DateTime.Now);
      }

      if (route.Count > 115)
      {
        Debug.Log("túlcsordult a biztonsági számláló");
        car.GetComponent<TweenHelper>().test = route.ToArray();
        return;
      }

    }
    Debug.Log("végzett a ciklus: " + route.Count);
    car.GetComponent<TweenHelper>().test = route.ToArray();
  }

  public GameObject nextElement()
  {
    GameObject theLast = route[route.Count - 1];
    RoadElementModel theLastScript = theLast.GetComponent<RoadElementModel>();

    if (theLastScript.NextElement != null)
    {
      Debug.Log(theLastScript.NextElement.name);
      return theLastScript.NextElement;
    }

    if (route.Count == 1)
    {
      //1 elem van, a forward-ot használjuk

      GameObject theNext = GetClosestElement(theLastScript.Position + forwardVector * 5);
      Debug.Log(theNext.name);
      theLastScript.NextElement = theNext;
      return theNext;
    }
    else
    {
      GameObject beforeLast = route[route.Count - 2];
      TransformModifier beforeLastScript = beforeLast.GetComponent<TransformModifier>();
      //Debug.Log(theLastScript.Position + "   ...   " + beforeLastScript.Position);
      Vector3 tempForward = (theLastScript.Position - beforeLastScript.Position).normalized;

      GameObject theNext = GetClosestElement(theLastScript.Position + tempForward * 5);
      //Debug.Log(theNext.name + "   ...   " + theNext.GetComponent<TransformModifier>().Position);

      if (theNext.name == theLast.name)
      {
        Debug.Log("bajvan: " + theNext.name);
        return GameObject.Find("POPCORN");
      }
      theLastScript.NextElement = theNext;
      return theNext;

    }

  }

}
