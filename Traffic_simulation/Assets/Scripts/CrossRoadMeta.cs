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

  public GameObject locker = null;

  public Queue<GameObject> waitRow;


	// Use this for initialization
	void Start()
	{
    waitRow = new Queue<GameObject>();
    locker = null;
  }

	// Update is called once per frame
	void Update()
	{
    if(locker == null && waitRow.Count != 0)
    {
      GameObject goForward = waitRow.Dequeue();
      goForward.GetComponent<TweenHelper>().speed = 10;
      locker = goForward;
    }
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

		float angleExit = Vector3.SignedAngle(from.transform.forward*(-1), exit.transform.forward, Vector3.up);

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
      Debug.Log("assymetric");
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

	public void CallGCalculationOnFromElements( GameObject from)
	{
		foreach (var item in roadGroup)
		{
			item.from.GetComponent<CrossRoadModel>().CalculateGValue(from);
		}
	}

	public void CallHCalculationOnExitElements( GameObject from, GameObject to)
	{
		foreach (var item in roadGroup)
		{
			foreach (var option in item.options)
			{
				CrossRoadModel.AStarCalc calcFrom = null;
				foreach (var calc in option.GetComponent<CrossRoadModel>().aStarValues)
				{
					if(GameObject.ReferenceEquals(calc.from, from))
					{
						calcFrom = calc;
					}
				}
				
				if (calcFrom == null || calcFrom.H.Equals(float.PositiveInfinity))
				{
					option.GetComponent<CrossRoadModel>().CalculateHValue(from,to);
				}
			}
		}
	}

  public void TriggerHandler(Collider other, GameObject road)
  {
    if (locker != null && !GameObject.ReferenceEquals(locker,other.gameObject))
    {
      other.gameObject.GetComponent<TweenHelper>().speed = 0;
      waitRow.Enqueue(other.gameObject);

      //StartCoroutine(WaitForMyStep(other));
    }
    if(locker == null)
    {
      foreach (var item in roadGroup)
      {
        if (GameObject.ReferenceEquals(item.from, road))
        {
          //bejáraton jött egy autó
          Debug.Log("bejáraton jött ");
          locker = other.gameObject;
        }
      }
    }
    if (GameObject.ReferenceEquals(locker, other.gameObject))
    {
      foreach (var item in roadGroup)
      {
        if (item.options.Contains(road))
        {
          //kimegy a kijáraton a lockoló autó
          Debug.Log("kijáraton távozik ");
          locker = null;
        }
      }
    }
    
  }

  IEnumerator WaitForMyStep(Collider other)
  {
    Debug.Log("Waiting for restart...");
    yield return new WaitUntil(() => locker != null);
    other.gameObject.GetComponent<TweenHelper>().speed = Random.Range(10000, 20000);
    Debug.Log("Restarted.");
  }


}
