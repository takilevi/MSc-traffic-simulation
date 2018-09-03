using System.Collections;
using System.Collections.Generic;
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

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public GameObject[] AddTheseToRoute(GameObject from, GameObject endObject, Vector3 forwardVector)
  {
    //1. ki kell találni mi ez a kezdőpont
    //2. a lehetséges kimenetelekből ki kell választani a LEGKÖZELEBBIT az endObject-hez képest
    //3. ha megvan a kijárat ki kell számolni, hogy az bal, jobb, vagy egyenes, és kiszámítani
    List<GameObject> options = GetOptions(from);
    Debug.Log("megvannak: " + options.Count);

    GameObject theExit = ClosestExitPoint(options, endObject);

    Children.Clear();
    foreach (Transform child in transform)
    {
      if (child.tag == "CrossRoad")
      {
        Children.Add(child.gameObject);
      }
    }
    Debug.Log("exit point: " + theExit);

    GameObject[] theRouteElements = GetDirectionObjects(from, theExit, forwardVector);

    return theRouteElements;
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

  GameObject ClosestExitPoint(List<GameObject> options, GameObject endObject)
  {
    if (options != null)
    {
      float dist = -1F;
      float distnew;
      GameObject tobe = null;
      for (int i = 0; i < options.Count; i++)
      {
        if (tobe == null)
        {
          dist = Vector3.Distance(endObject.GetComponent<TransformModifier>().Position, options[i].GetComponent<TransformModifier>().Position);
          tobe = options[i];
          Debug.Log("init....  " + tobe.name + "    ..  " + dist);
          continue;
        }

        distnew = Vector3.Distance(endObject.GetComponent<TransformModifier>().Position, options[i].GetComponent<TransformModifier>().Position);
        
        if (distnew < dist)
        {
          dist = distnew;
          tobe = options[i];
        }
        Debug.Log("....  " + options[i].name + "    ..  " + distnew + ".... " + dist);

      }

      return tobe;
    }

    return null;

  }

  GameObject[] GetDirectionObjects(GameObject from, GameObject exit, Vector3 forwardVector)
  {
    /*Debug.Log("forward: " + forwardVector);
    float angleRight = Vector3.SignedAngle(forwardVector, (GameObject.Find("SM_Env_Road_Crossing_11").GetComponent<TransformModifier>().Position - from.GetComponent<TransformModifier>().Position).normalized, Vector3.up);
    float angleForward = Vector3.SignedAngle(forwardVector, (GameObject.Find("SM_Env_Road_Crossing_13").GetComponent<TransformModifier>().Position - from.GetComponent<TransformModifier>().Position).normalized, Vector3.up);
    float angleLeft = Vector3.SignedAngle(forwardVector, (GameObject.Find("SM_Env_Road_Crossing_32").GetComponent<TransformModifier>().Position - from.GetComponent<TransformModifier>().Position).normalized, Vector3.up);
    */
    float angleExit = Vector3.SignedAngle(forwardVector, (exit.GetComponent<TransformModifier>().Position - from.GetComponent<TransformModifier>().Position).normalized, Vector3.up);
    Debug.Log("kanyarodási szög: " + angleExit);
    GameObject[] theArray;
    if (angleExit > 40f)
    {
      theArray = RightPath(from, forwardVector);
    }
    else if(angleExit < -40f)
    {
      theArray = LeftPath(from, forwardVector);
    }
    else
    {
      theArray = ForwardPath(from, forwardVector);
    }

    foreach (var item in theArray)
    {
      Debug.Log(item.name);
    }
    return theArray;
  }

  private GameObject[] RightPath(GameObject roadItem, Vector3 direction)
  {
    Vector3 rotatedVectorBy90 = Quaternion.Euler(0, 90, 0) * direction;
    List<GameObject> rightPath = new List<GameObject>();
    Vector3 newInitialPoint = roadItem.GetComponent<TransformModifier>().Position;
    Vector3 forward = direction * 5;

    rightPath.Add(roadItem);

    for (int i = 0; i < 2; i++)
    {
      foreach (var item in Children)
      {
        GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

        if (!temp.Equals(rightPath[rightPath.Count - 1]))
        {
          newInitialPoint = temp.GetComponent<TransformModifier>().Position;
          if (i == 0)
          {
            forward = rotatedVectorBy90*5;
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
    Vector3 newInitialPoint = roadItem.GetComponent<TransformModifier>().Position;
    Vector3 forward = direction * 5;

    leftPath.Add(roadItem);

    for (int i = 0; i < 4; i++)
    {
      foreach (var item in Children)
      {
        GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

        if (!temp.Equals(leftPath[leftPath.Count - 1]))
        {
          newInitialPoint = temp.GetComponent<TransformModifier>().Position;
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
    Vector3 newInitialPoint = roadItem.GetComponent<TransformModifier>().Position;
    Vector3 forward = direction*5;

    forwardPath.Add(roadItem);

    for (int i = 0; i < 4; i++)
    {
      foreach (var item in Children)
      {
        GameObject temp = GetClosestInsideCrossElement(newInitialPoint + forward);

        if (!temp.Equals(forwardPath[forwardPath.Count - 1]))
        {
          forwardPath.Add(temp);
          newInitialPoint = temp.GetComponent<TransformModifier>().Position;
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
      float dist = Vector3.Distance(possiblePos, theChoosenOne.GetComponent<TransformModifier>().Position);
      float newDist = Vector3.Distance(possiblePos, item.GetComponent<TransformModifier>().Position);

      if (newDist < dist)
      {
        theChoosenOne = item;
      }

    }
    return theChoosenOne;
  }
}
