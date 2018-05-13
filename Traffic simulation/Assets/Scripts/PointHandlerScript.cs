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
    allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadElement");
    allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
    allRoad = new List<GameObject>();
    allRoad.AddRange(allRoadGameObject);
    allRoad.AddRange(allCrossRoadGameObject);

    route = new List<GameObject>();

    PathPointRecreateAndNull();

    PathCalculator();

    if(car !=null && popcorn != null && closestObject != null && endObject != null && forwardVector != null)
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
      Vector3 nullvec = new Vector3(0f, 0f, 0f);
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
      if(nextOne.tag == "RoadElement")
      {
        nextOne = nextElement();
      }
      if(nextOne.tag == "CrossRoad")
      {
        CrossRoadMeta parentMeta = nextOne.GetComponentInParent<CrossRoadMeta>();

        //amit a függvény visszaad ahhoz még hozzá kell csapni a kereszteződés utáni első roadelementet!!!
        GameObject[] addables = parentMeta.AddTheseToRoute(nextOne, endObject, 
          (nextOne.GetComponent<TransformModifier>().Position 
                - route[route.Count-1].GetComponent<TransformModifier>().Position).normalized);

        route.AddRange(addables);

        /*foreach (var item in route)
        {
          Debug.Log("route elements: " + item.name);
        }
        */

        Vector3 newForward = (route[route.Count - 1].GetComponent<TransformModifier>().Position
                - route[route.Count - 2].GetComponent<TransformModifier>().Position).normalized;

        GameObject theNext = GetClosestElement(route[route.Count - 1].GetComponent<TransformModifier>().Position + newForward * 5);
        Debug.Log("kereszteződés utáni elem: "+theNext.name+" ......   "+newForward);

        nextOne = theNext;

      }

      if (route.Count > 50)
      {
        Debug.Log("route szám miatt ugrottunk ki");
        return;
      }
      
    }
    Debug.Log("végzett a ciklus");
    //ha a nextElement roadelement akkor megyünk tovább, ha crossroad akkor külön kezeljük
  }

  public GameObject nextElement()
  {
    GameObject theLast = route[route.Count - 1];
    RoadElementModel theLastScript = theLast.GetComponent<RoadElementModel>();

    if(theLastScript.NextElement != null)
    {
      Debug.Log(theLastScript.NextElement.name);
      return theLastScript.NextElement;
    }

    if (route.Count == 1)
    {
      //1 elem van, a forward-ot használjuk

      GameObject theNext = GetClosestElement(theLastScript.Position + forwardVector * 5);
      Debug.Log(theNext.name);
      return theNext;
    }
    else
    {
      GameObject beforeLast = route[route.Count - 2];
      TransformModifier beforeLastScript = beforeLast.GetComponent<TransformModifier>();
      Debug.Log(theLastScript.Position + "   ...   " + beforeLastScript.Position);
      Vector3 tempForward = (theLastScript.Position - beforeLastScript.Position).normalized;

      GameObject theNext = GetClosestElement(theLastScript.Position + tempForward * 5);
      Debug.Log(theNext.name + "   ...   " + theNext.GetComponent<TransformModifier>().Position);

      return theNext;

    }
    
  }

}
