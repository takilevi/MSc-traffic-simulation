using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPathController : MonoBehaviour
{

  public List<Transform> allTransform = new List<Transform>();
  public GameObject[] allRoadGameObject = null;
  public GameObject[] allCrossRoadGameObject = null;
  public GameObject car;
  public List<Transform> onePath = new List<Transform>();
  public List<Transform[]> alternativePaths = new List<Transform[]>();
  public Color[] alternativePathsColorEnum = new Color[] { Color.red, Color.blue, Color.green };
  private int selectedWay;

  private bool firstLoad = true;
  private bool firstLoadGizmos = true;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

    if (firstLoad)
    {
      if (onePath.Count == 0)
      {
        Debug.Log("Nincs még nyomvonal");
        LogDraw();
      }
      else
      {
        firstLoad = false;
        StartCar();
      }
    }
  }

  public void StartCar()
  {
    selectedWay = Random.Range(0, alternativePaths.Count);
    iTween.MoveTo(gameObject,
         iTween.Hash("path", alternativePaths[selectedWay], "time", 20, "orienttopath", true,
         "looktime", .6, "easetype", iTween.EaseType.linear, "onComplete", "FurtherPath"));
  }

  public void FurtherPath()
  {
    Debug.Log("folytatás");
    Debug.Log(car.name);
    Debug.Log(car.transform.forward);


    Transform closestRoadElement = null;

    foreach (var item in allTransform)
    {
      if (closestRoadElement == null ||
          (Vector3.Distance(car.transform.position, item.transform.position)) <
          (Vector3.Distance(closestRoadElement.transform.position, item.transform.position)))
      {
        closestRoadElement = item;
      }
    }

    Dictionary<CrossRoadHelper.AvailableDirections, Transform[]> alternativeWays = findMorePath(closestRoadElement,0);

    if (alternativeWays != null)
    {
      foreach (var alternative in alternativeWays)
      {
        switch (alternative.Key)
        {
          case CrossRoadHelper.AvailableDirections.Forward:

            /************************************************************************
             * 
             * a forward lesz az alapértelmezett !!!
             * az alternative.Value lesz az út amit hozzáfűzünk a MoveTo-hoz
             * 
             ************************************************************************/

            List<Transform> forwardAlternative = new List<Transform>(alternativePaths[selectedWay]);

            forwardAlternative.AddRange(alternative.Value);

            alternativePaths[selectedWay] = forwardAlternative.ToArray();

            iTween.MoveTo(gameObject,
                iTween.Hash("path", alternative.Value, "time", 20, "orienttopath", true,
                "looktime", .6, "easetype", iTween.EaseType.linear));
            break;
          case CrossRoadHelper.AvailableDirections.Right:
            /*
             * TODO
             */
            break;
          case CrossRoadHelper.AvailableDirections.Left:
            /*
             * TODO
             */
            break;
          default:
            break;
        }
      }
    }

  }
  private Dictionary<CrossRoadHelper.AvailableDirections, Transform[]> findMorePath(Transform colestRoadElement, int moreCrosses)
  {
    Dictionary<CrossRoadHelper.AvailableDirections, Transform[]> morePathDictionary =
          new Dictionary<CrossRoadHelper.AvailableDirections, Transform[]>();
    Transform newInitialPoint = colestRoadElement;
    Vector3 forward = new Vector3(car.transform.forward.x * 5, car.transform.forward.y * 5, car.transform.forward.z * 5);
    List<Transform> morePath = new List<Transform>();

    for (int i = 0; i < allTransform.Count; i++)
    {
      foreach (var item in allTransform)
      {
        float distance = Vector3.Distance(newInitialPoint.transform.position + forward, item.transform.position);
        if (distance < 4f && !newInitialPoint.transform.Equals(item.transform))
        {
          /*  Debug.Log("a target point: " + (newInitialPoint.transform.position + forward) +
                        "    distance: " + distance + "    forward: " + forward + "    a megtalált pathpoint: " + item.transform.position);
          */
          if (distance > 0)
          {
            Vector3 dir = (item.transform.position - newInitialPoint.transform.position).normalized;
            forward = new Vector3(dir.x * 4, dir.y * 4, dir.z * 4);
            Debug.Log("módosított forward: " + forward);
          }

          newInitialPoint = item.transform;

          var crossItem = IsRoadCrossItem(item.transform, forward.normalized);
          if (crossItem != null && moreCrosses>0)
          {
            Debug.Log("ez egy roadcross item: " + item.name);
            return crossItem.GetComponent<CrossRoadHelper>().FindAlternativesForItem(item.transform, forward.normalized);

          }
          morePath.Add(item.transform);
          break;

        }
      }
    }

    morePathDictionary.Add(CrossRoadHelper.AvailableDirections.Forward,morePath.ToArray());
    return morePathDictionary;
  }
  private void LogDraw()
  {
    allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadElement");
    allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");

    //GameObject.ReferenceEquals(firstGameObject, secondGameObject)
    allTransform.Clear();

    foreach (var item in allRoadGameObject)
    {
      allTransform.Add(item.transform);
    }

    //Debug.Log(allTransform.Count);

    Transform closestRoadElement = null;

    foreach (var item in allTransform)
    {
      if (closestRoadElement == null ||
          (Vector3.Distance(car.transform.position, item.transform.position)) <
          (Vector3.Distance(closestRoadElement.transform.position, item.transform.position)))
      {
        closestRoadElement = item;
      }
    }

    onePath.Add(closestRoadElement);

    Dictionary<CrossRoadHelper.AvailableDirections, Transform[]> alternativeWays = PathFromPoint(closestRoadElement);
    if (alternativeWays != null)
    {
      foreach (var alternative in alternativeWays)
      {
        switch (alternative.Key)
        {
          case CrossRoadHelper.AvailableDirections.Forward:
            List<Transform> forwardAlternative = new List<Transform>(onePath);
            forwardAlternative.AddRange(alternative.Value);
            alternativePaths.Add(forwardAlternative.ToArray());
            break;
          case CrossRoadHelper.AvailableDirections.Right:
            List<Transform> rightAlternative = new List<Transform>(onePath);
            rightAlternative.AddRange(alternative.Value);
            alternativePaths.Add(rightAlternative.ToArray());
            break;
          case CrossRoadHelper.AvailableDirections.Left:
            List<Transform> leftAlternative = new List<Transform>(onePath);
            leftAlternative.AddRange(alternative.Value);
            alternativePaths.Add(leftAlternative.ToArray());
            break;
          default:
            break;
        }
      }
    }



  }

  private Dictionary<CrossRoadHelper.AvailableDirections, Transform[]> PathFromPoint(Transform initialPoint)
  {
    Transform newInitialPoint = initialPoint;
    Vector3 forward = new Vector3(car.transform.forward.x * 5, car.transform.forward.y * 5, car.transform.forward.z * 5);

    for (int i = 0; i < allTransform.Count; i++)
    {
      foreach (var item in allTransform)
      {
        float distance = Vector3.Distance(newInitialPoint.transform.position + forward, item.transform.position);
        if (distance < 4f && !newInitialPoint.transform.Equals(item.transform))
        {
          /*  Debug.Log("a target point: " + (newInitialPoint.transform.position + forward) +
                        "    distance: " + distance + "    forward: " + forward + "    a megtalált pathpoint: " + item.transform.position);
          */
          if (distance > 0)
          {
            Vector3 dir = (item.transform.position - newInitialPoint.transform.position).normalized;
            forward = new Vector3(dir.x * 4, dir.y * 4, dir.z * 4);
            Debug.Log("módosított forward: " + forward);
          }

          newInitialPoint = item.transform;

          var crossItem = IsRoadCrossItem(item.transform, forward.normalized);
          if (crossItem != null)
          {
            Debug.Log("ez egy roadcross item: " + item.name);
            return crossItem.GetComponent<CrossRoadHelper>().FindAlternativesForItem(item.transform, forward.normalized);

          }
          onePath.Add(item.transform);
          break;

        }
      }
    }

    alternativePaths.Add(onePath.ToArray());
    Debug.Log("onepath count: " + onePath.Count);
    return null;
  }

  private GameObject IsRoadCrossItem(Transform roadItem, Vector3 direction)
  {
    foreach (var item in allCrossRoadGameObject)
    {
      List<GameObject> crossItems = item.GetComponent<CrossRoadHelper>().GetAllChildren();
      foreach (var cross in crossItems)
      {
        if (GameObject.ReferenceEquals(cross, roadItem.gameObject))
        {
          return item;
        }
      }

    }
    return null;
  }

  private void OnDrawGizmos()
  {
    if (alternativePaths.Count == 0)
    {
      Debug.Log("OnDraw függvény, nincs nyomvonal");
      //LogDraw();
    }

    Debug.Log("OnDrawGizmos-ban töltöttük: " + alternativePaths.Count);
    for (int i = 0; i < alternativePaths.Count; i++)
    {
      iTween.DrawPath(alternativePaths[i], alternativePathsColorEnum[i]);
    }

  }
}
