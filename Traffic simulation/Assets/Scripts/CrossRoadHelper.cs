using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoadHelper : MonoBehaviour
{

  public List<GameObject> Children;
  public enum AvailableDirections { Forward, Right, Left };
  public bool forwardAvailable;
  public bool rightAvailable;
  public bool leftAvailable;
  public GameObject inObj;
  public Vector3 direction;


  void Start()
  {
    foreach (Transform child in transform)
    {
      if (child.tag == "CrossRoad")
      {
        Children.Add(child.gameObject);
      }
    }
  }

  public List<GameObject> GetAllChildren()
  {
    return Children;
  }

  public Dictionary<AvailableDirections, Transform[]> FindAlternativesForItem(Transform roadItem, Vector3 direction)
  {
    Dictionary<AvailableDirections, Transform[]> alternatives = new Dictionary<AvailableDirections, Transform[]>();
    if (forwardAvailable)
    {
      alternatives.Add(AvailableDirections.Forward, ForwardPath(roadItem, direction));
    }
    if (rightAvailable)
    {
      alternatives.Add(AvailableDirections.Right, RightPath(roadItem, direction));
    }
    if (leftAvailable)
    {
      alternatives.Add(AvailableDirections.Left, LeftPath(roadItem, direction));
    }

    /*
    foreach (KeyValuePair<AvailableDirections, Transform[]> entry in alternatives)
    {
      foreach (var item in entry.Value)
      {
        Debug.Log("Key: " + entry.Key + "    Value: " + item.transform.position);
      }
    }
    */
    return alternatives;
  }

  private Transform[] ForwardPath(Transform roadItem, Vector3 direction)
  {
    List<Transform> forwardPath = new List<Transform>();
    Transform newInitialPoint = roadItem;
    Vector3 forward = new Vector3(direction.x * 5, direction.y * 5, direction.z * 5);
    forwardPath.Add(roadItem);

    for (int i = 0; i < Children.Count; i++)
    {
      foreach (var item in Children)
      {
        float distance = Vector3.Distance(newInitialPoint.transform.position + forward, item.transform.position);
        if (distance < 4f && !newInitialPoint.transform.Equals(item.transform))
        {
          newInitialPoint = item.transform;
          forwardPath.Add(item.transform);

          break;

        }
      }
    }

    return forwardPath.ToArray();
  }

  private Transform[] RightPath(Transform roadItem, Vector3 direction)
  {
    Vector3 rotatedVectorBy90 = Quaternion.Euler(0, 90, 0) * direction;
    List<Transform> rightPath = new List<Transform>();
    Transform newInitialPoint = roadItem;
    Vector3 forward = new Vector3(direction.x * 5, direction.y * 5, direction.z * 5);
    rightPath.Add(roadItem);

    for (int i = 0; i < 2; i++)
    {
      foreach (var item in Children)
      {

        float distance = Vector3.Distance((newInitialPoint.transform.position + forward), item.transform.position);
        if (distance <= 1f && !newInitialPoint.transform.Equals(item.transform))
        {
          newInitialPoint = item.transform;
          if (i == 0)
          {
            forward = new Vector3(rotatedVectorBy90.x * 5, rotatedVectorBy90.y * 5, rotatedVectorBy90.z * 5);
            rightPath.Add(item.transform);
          }
          else
          {
            rightPath.Add(item.transform);
          }

          break;

        }
      }
    }
    return rightPath.ToArray();
  }

  private Transform[] LeftPath(Transform roadItem, Vector3 direction)
  {
    Vector3 rotatedVectorBy90 = Quaternion.Euler(0, -90, 0) * direction;
    List<Transform> leftPath = new List<Transform>();
    Transform newInitialPoint = roadItem;
    Vector3 forward = new Vector3(direction.x * 5, direction.y * 5, direction.z * 5);
    leftPath.Add(roadItem);

    for (int i = 0; i < 4; i++)
    {
      foreach (var item in Children)
      {
        float distance = Vector3.Distance(newInitialPoint.transform.position + forward, item.transform.position);
        if (distance <= 1f && !newInitialPoint.transform.Equals(item.transform))
        {
          newInitialPoint = item.transform;
          if (i == 1)
          {
            forward = new Vector3(rotatedVectorBy90.x * 5, rotatedVectorBy90.y * 5, rotatedVectorBy90.z * 5);
            break;
          }
          else
          {
            leftPath.Add(item.transform);
          }

          break;

        }
      }
    }

    return leftPath.ToArray();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
