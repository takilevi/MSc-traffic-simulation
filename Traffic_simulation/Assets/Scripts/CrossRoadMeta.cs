using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class CrossRoadMeta : MonoBehaviour
{

  [System.Serializable]
  public class RoadGroup
  {
    public GameObject from;
    public List<GameObject> options;
  }
  public RoadGroup[] roadGroup;

  public List<GameObject> Children;

  public Dictionary<Collider, GameObject> waitRow;

  public Dictionary<GameObject, GameObject> crossLock;


  // Use this for initialization
  void Start()
  {
    waitRow = new Dictionary<Collider, GameObject>();
    crossLock = new Dictionary<GameObject, GameObject>();
    foreach (var item in Children)
    {
      crossLock.Add(item, null);
    }
  }

  // Update is called once per frame
  void Update()
  {
    /*
    if(this.gameObject.name== "CrossRoad_4_arm (2)")
    {
      foreach (var item in waitRow)
      {
        Debug.Log("waitRow:  Key (collider): " + item.Key.gameObject.name + "\tValue (car): " + item.Value.gameObject.name);
      }
      Debug.Log("-----------------------------------------------");

      foreach (var item in crossLock)
      {
        if(item.Value != null)
          Debug.Log("crossLock:  Key (cross): " + item.Key.gameObject.name + "\tValue (car): " + item.Value.gameObject.name);
      }
      Debug.Log("-----------------------------------------------");
    }
    */
    
  }



  public List<GameObject> GetOptions(GameObject from)
  {
    foreach (var item in roadGroup)
    {
      if (item.from.Equals(from))
      {
        return item.options;
      }
    }

    return null;
  }


  public GameObject[] GetDirectionObjects(GameObject from, GameObject exit)
  {
    Children.Clear();
    foreach (Transform item in transform)
    {
      Children.Add(item.gameObject);
    }

    float angleExit = Vector3.SignedAngle(from.transform.forward * (-1), exit.transform.forward, Vector3.up);

    //Debug.Log("kanyarodási szög: " + angleExit);

    GameObject[] theArray;
    if (angleExit > 40f)
    {
      theArray = RightPath(from, from.transform.forward * (-1));
    }
    else if (angleExit < -40f)
    {
      theArray = LeftPath(from, from.transform.forward * (-1));
    }
    else
    {
      theArray = ForwardPath(from, from.transform.forward * (-1));
    }
    return theArray;
  }

  private GameObject[] RightPath(GameObject roadItem, Vector3 direction)
  {
    Vector3 rotatedVectorBy90 = Quaternion.Euler(0, 90, 0) * direction;
    List<GameObject> rightPath = new List<GameObject>();
    Vector3 newInitialPoint = roadItem.transform.position;
    Vector3 forward = direction * 5;

    rightPath.Add(roadItem);

    for (int i = 0; i < 2; i++)
    {
      foreach (var item in Children)
      {
        GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

        if (!temp.Equals(rightPath[rightPath.Count - 1]))
        {
          newInitialPoint = temp.transform.position;
          if (i == 0)
          {
            forward = rotatedVectorBy90 * 5;
            rightPath.Add(temp);
            break;
          }
          else
          {
            rightPath.Add(temp);
          }

          break;
        }
      }
    }

    return rightPath.ToArray();
  }
  private GameObject[] LeftPath(GameObject roadItem, Vector3 direction)
  {
    Vector3 rotatedVectorBy90 = Quaternion.Euler(0, -90, 0) * direction;
    List<GameObject> leftPath = new List<GameObject>();
    Vector3 newInitialPoint = roadItem.transform.position;
    Vector3 forward = direction * 5;

    leftPath.Add(roadItem);

    for (int i = 0; i < 4; i++)
    {
      foreach (var item in Children)
      {
        GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

        if (!temp.Equals(leftPath[leftPath.Count - 1]))
        {
          newInitialPoint = temp.transform.position;
          if (i == 1)
          {
            forward = rotatedVectorBy90 * 5;
            leftPath.Add(temp);
            break;
          }
          else
          {
            leftPath.Add(temp);
          }

          break;
        }
      }
    }

    return leftPath.ToArray();
  }

  private GameObject[] ForwardPath(GameObject roadItem, Vector3 direction)
  {
    List<GameObject> forwardPath = new List<GameObject>();
    Vector3 newInitialPoint = roadItem.transform.position;
    Vector3 forward = direction * 5;

    forwardPath.Add(roadItem);

    if (this.gameObject.name.Contains("Assymetric"))
    {
      //Debug.Log("assymetric");
      Vector3 rotatedVectorBy45 = Quaternion.Euler(0, 45, 0) * direction;
      forward = rotatedVectorBy45 * 7;

      GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

      if (!temp.Equals(forwardPath[forwardPath.Count - 1]))
      {
        forwardPath.Add(temp);
      }
    }

    else
    {
      for (int i = 0; i < 4; i++)
      {
        foreach (var item in Children)
        {
          GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

          if (!temp.Equals(forwardPath[forwardPath.Count - 1]))
          {
            forwardPath.Add(temp);
            newInitialPoint = temp.transform.position;
          }
        }
      }
    }

    return forwardPath.ToArray();
  }

  public GameObject GetClosestInsideCrossElement(Vector3 possiblePos)
  {
    GameObject theChoosenOne = null;
    foreach (var item in Children)
    {
      if (theChoosenOne == null)
      {
        theChoosenOne = item;
      }
      float dist = Vector3.Distance(possiblePos, theChoosenOne.transform.position);
      float newDist = Vector3.Distance(possiblePos, item.transform.position);

      if (newDist < dist)
      {
        theChoosenOne = item;
      }

    }
    return theChoosenOne;
  }

  public List<GameObject> PossibleEntrancesToExit(GameObject exit)
  {
    List<GameObject> entrances = new List<GameObject>();

    foreach (var item in roadGroup)
    {
      if (item.options.Contains(exit))
      {
        entrances.Add(item.from);
      }
    }

    return entrances;
  }

  public void CallGCalculationOnFromElements(GameObject from)
  {
    foreach (var item in roadGroup)
    {
      item.from.GetComponent<CrossRoadModel>().CalculateGValue(from);
    }
  }

  public void CallHCalculationOnExitElements(GameObject from, GameObject to)
  {
    foreach (var item in roadGroup)
    {
      foreach (var option in item.options)
      {
        CrossRoadModel.AStarCalc calcFrom = null;
        foreach (var calc in option.GetComponent<CrossRoadModel>().aStarValues)
        {
          if (GameObject.ReferenceEquals(calc.from, from))
          {
            calcFrom = calc;
          }
        }

        if (calcFrom == null || calcFrom.H.Equals(float.PositiveInfinity))
        {
          option.GetComponent<CrossRoadModel>().CalculateHValue(from, to);
        }
      }
    }
  }


  public bool TriggerHandler(Collider other, GameObject road)
  {
    bool lockSuccess = false;
    if (RoadIsEntry(road))
    {
      List<GameObject> crossItems = other.gameObject.GetComponent<DiscoverNeighbours>().GetGameObjectsInCross(road);
      bool isCrossElementsLocked = CrossElementsLocked(crossItems);

      if (!isCrossElementsLocked)
      {
        //lockolás
        foreach (var crossItem in crossItems)
        {
          crossLock[crossItem] = other.gameObject;
        }
        if (Mathf.Approximately(other.gameObject.GetComponent<TweenHelper>().currentSpeed,0.0f))
        {
          other.gameObject.GetComponent<TweenHelper>().RestoreToInitialSpeed();
          other.gameObject.GetComponent<CarController>().carInWaitRow = false;
        }
        lockSuccess = true;
        other.gameObject.GetComponent<CarController>().carInCross = true;

      }

      if (isCrossElementsLocked)
      {
        other.gameObject.GetComponent<TweenHelper>().SetCurrentSpeed(0.0f);
        other.gameObject.GetComponent<CarController>().carInWaitRow = true;
        if (!waitRow.ContainsKey(other))
        {
          waitRow.Add(other, road);
        }
      }
    }
    return lockSuccess;
  }

  public bool CrossElementsLocked(List<GameObject> crossItems)
  {
    foreach (var crossItem in crossItems)
    {
      foreach (var lockItem in crossLock)
      {
        if (GameObject.ReferenceEquals(crossItem, lockItem.Key) && lockItem.Value != null)
        {
          return true;
        }
      }
    }
    return false;
  }

  public bool RoadIsEntry(GameObject road)
  {
    foreach (var item in roadGroup)
    {
      if (GameObject.ReferenceEquals(item.from, road))
      {
        //bejáraton jött egy autó
        return true;
      }
    }

    return false;
  }

  public void TriggerHandlerExit(Collider other, GameObject road)
  {
    List<GameObject> lockObj = new List<GameObject>();
    if (RoadIsExit(road))
    {
      //unlock where the 'other' is the locker
      foreach (var item in crossLock.Keys)
      {
        if (GameObject.ReferenceEquals(other.gameObject, crossLock[item]))
        {
          lockObj.Add(item);
        }
      }
      if(lockObj.Count != 0)
      {
        other.gameObject.GetComponent<CarController>().carInCross = false;
      }
      foreach (var item in lockObj)
      {
        crossLock[item] = null;
      }
    }

    if (lockObj.Count > 0)
    {
      CheckWaitRow();
    }
  }

  public bool RoadIsExit(GameObject road)
  {
    foreach (var item in roadGroup)
    {
      if (item.options.Contains(road))
      {
        //kijáraton kimegy egy autó
        return true;
      }
    }

    return false;
  }

  private void CheckWaitRow()
  {
    List<Collider> toRemoveList = new List<Collider>();

    if (waitRow.Count != 0)
    {
      foreach (var item in waitRow)
      {
        bool carCanGo = TriggerHandler(item.Key, item.Value);
        if(carCanGo)
        {
          toRemoveList.Add(item.Key);
        }
      }
    }

    foreach (var item in toRemoveList)
    {
      waitRow.Remove(item);
    }
  }


}
