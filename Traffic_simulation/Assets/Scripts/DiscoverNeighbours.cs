using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoverNeighbours : MonoBehaviour
{

	public GameObject[] allRoadGameObject;
	public GameObject[] allCrossRoadGameObject;
	public CrossRoadMeta[] crossMetaObject;

	public List<GameObject> calculatedRoute;
  public GameObject from;
  public GameObject to;

	public class AStarNode
	{
		public GameObject node;
		public AStarNode parent;
		public float F;

		public AStarNode(GameObject node, AStarNode parent, float f)
		{
			this.node = node;
			this.parent = parent;
			F = f;
		}
	}
	public Queue<AStarNode> openList;
	public Queue<AStarNode> closedList;
	public Queue<GameObject> aStarResult;

	void Awake()
	{
    ElementTable.Setup();

    this.GetComponent<TweenHelper>().test = null;
		this.GetComponent<TweenHelper>().testV3 = null;

		calculatedRoute = new List<GameObject>();
		openList = new Queue<AStarNode>();
		closedList = new Queue<AStarNode>();
		aStarResult = new Queue<GameObject>();

    from = ElementTable.GetRandomSource(this.gameObject);
		to = ElementTable.GetRandomDestination();
		
		crossMetaObject = GameObject.FindObjectsOfType<CrossRoadMeta>();

    PlaceCarToStartingPoint(from);
		FindShortestPath(from, to);
	}

 

  // Update is called once per frame
  void Update()
	{

	}

  private void PlaceCarToStartingPoint(GameObject from)
  {
    this.gameObject.transform.position = from.transform.position;
    this.gameObject.transform.rotation = Quaternion.LookRotation(from.transform.forward);
  }

  public void FindShortestPath(GameObject from, GameObject to)
	{
		foreach (var item in crossMetaObject)
		{
			//A* -hoz szükséges számítások
			//csúcspontok súlyozása G-H-F értékek
			item.CallGCalculationOnFromElements(this.gameObject);
			item.CallHCalculationOnExitElements(this.gameObject, to);
		}

		calculatedRoute.Add(from);

		bool straightRoad = true;
		List<GameObject> exitsFromStart = new List<GameObject>();
		while (straightRoad)
		{
			GameObject nextElement = calculatedRoute[calculatedRoute.Count - 1].GetComponent<RoadElementModel>().nextElement;

			if (GameObject.ReferenceEquals(nextElement, to))
			{
				//Debug.Log("megvagyunk");
				calculatedRoute.Add(nextElement);
				return;
			}
			Component nextComp = nextElement.GetComponent<CrossRoadModel>();
			if (nextComp != null)
			{
				exitsFromStart = nextComp.GetComponentInParent<CrossRoadMeta>().GetOptions(nextElement);
				straightRoad = false;

				foreach (var item in exitsFromStart)
				{
					openList.Enqueue(new AStarNode(item, null, calculatedRoute.Count + item.GetComponent<CrossRoadModel>().GetH(this.gameObject)));
					openList = new Queue<AStarNode>(openList.OrderBy(x => x.F));
				}

			}
			calculatedRoute.Add(nextElement);


		}

		//ha megvan az elem akkor már kiléptünk
		//ha megvan a legközelebbi kereszteződés akkor ide jutottunk, a lehetséges kimenetelek a exits változóban

		bool searchNearestCrossToEnd = true;
		GameObject prevUntilClosestCross = to;
		while (searchNearestCrossToEnd)
		{
			GameObject prevElement = prevUntilClosestCross.GetComponent<RoadElementModel>().previousElement;

			Component prevComp = prevElement.GetComponent<CrossRoadModel>();
			if (prevComp != null)
			{
				searchNearestCrossToEnd = false;
			}

			prevUntilClosestCross = prevElement;
		}


		//megvannak a CÉL legközelebbi entrance pontjai
		//innen kezdődik maga az algoritmus

		/*
     * Debug.Log("Open list :\n " + String.Join("",
						 new List<AStarNode>(openList)
						 .ConvertAll(i => String.Concat(i.node.ToString(), i.node.transform.parent.ToString(), "\t", i.F, "\n"))
						 .ToArray()));
     */


		do
		{
			closedList.Enqueue(openList.Dequeue());
			if (GameObject.ReferenceEquals(prevUntilClosestCross, closedList.ToList().Last().node))
			{
				//path found
				//Debug.Log("Found the exit point, from where we will reach the end object");

				break;
			}

			GameObject adjacentEntrance = closedList.ToList().Last().node.GetComponent<CrossRoadModel>().canConnectToFromExit;
			List<GameObject> adjacentExits = adjacentEntrance.GetComponentInParent<CrossRoadMeta>().GetOptions(adjacentEntrance);
			List<AStarNode> adjacentAStarNodes = new List<AStarNode>();
			foreach (var item in adjacentExits)
			{
				adjacentAStarNodes.Add(new AStarNode(item, closedList.ToList().Last(), closedList.ToList().Last().F + adjacentEntrance.GetComponent<CrossRoadModel>().GetG(this.gameObject) 
					+ item.GetComponent<CrossRoadModel>().GetH(this.gameObject)));
			}

			foreach (var aSquare in adjacentAStarNodes)
			{
				//megnézzük hogy benne van e már a closedlistben
				bool flag = false;
				foreach (var closed in closedList)
				{
					if (GameObject.ReferenceEquals(closed.node, aSquare.node))
					{
						flag = true;
						break;
					}
				}
				if (flag) continue;

				AStarNode oldOpen = null;
				foreach (var open in openList)
				{
					if (GameObject.ReferenceEquals(open.node, aSquare.node))
					{
						oldOpen = open;
					}
				}
				if (oldOpen == null)
				{
					openList.Enqueue(aSquare);
				}
				else
				{
					//Recalculate the parent and the F value!!!

					float oldF = oldOpen.F;
					float newF = aSquare.F;

					//Debug.Log("oldF: " + oldF + " ------ newF: " + newF);
					if (newF < oldF)
					{
						//Debug.Log("oldF: " + oldF + " ------ newF: " + newF + "   -----whoami- " + String.Concat(oldOpen.node.name, oldOpen.node.transform.parent.name));
						//Debug.Log("bejöttünk a newF kisebb oldF ágba ---- oldparent : " + oldOpen.parent.node.name + " ----  " + oldOpen.parent.node.transform.parent + "   --- newparent: " + closedList.ToList().Last().node + "  -----  " + closedList.ToList().Last().node.transform.parent);
						oldOpen.F = newF;
						oldOpen.parent = closedList.ToList().Last();
					}
				}

				openList = new Queue<AStarNode>(openList.OrderBy(x => x.F));
			}

			/*
       * https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
			 * 
			 * */

		} while (openList.Any());

		//kész az útkeresés, ez a csúcsok közti kapcsolat térképe
		AStarNode nextReverse = closedList.ToList().Last();
		do
		{
			aStarResult.Enqueue(nextReverse.node);
			nextReverse = nextReverse.parent;

		} while (nextReverse != null);

		aStarResult = new Queue<GameObject>(aStarResult.Reverse());

    /*
    Debug.Log("REVERSE A* pathfinding result :\n " + String.Join("",
						 new List<GameObject>(aStarResult)
						 .ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(), "\n"))
						 .ToArray()));
    */

		bool endNotReached = true;
		while (endNotReached)
		{
			GameObject nextcross = aStarResult.Dequeue();


			calculatedRoute.AddRange(calculatedRoute.Last().GetComponentInParent<CrossRoadMeta>().GetDirectionObjects(calculatedRoute.Last(), nextcross));
			calculatedRoute = calculatedRoute.Distinct().ToList();

			/*
			Debug.Log("útvonal DISTINCT :\n " + String.Join("",
               new List<GameObject>(calculatedRoute)
               .ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(), "\n"))
               .ToArray()));
			*/
               

			calculatedRoute.Add(calculatedRoute.Last().GetComponent<CrossRoadModel>().closestRoad);

			bool insideRoads = true;
			do
			{
				if (GameObject.ReferenceEquals(calculatedRoute.Last(), to))
				{
					endNotReached = false;
				}
				else if (calculatedRoute.Last().GetComponent<CrossRoadModel>() != null)
				{
					insideRoads = false;
				}
				else
				{
					calculatedRoute.Add(calculatedRoute.Last().GetComponent<RoadElementModel>().nextElement);
				}

			} while (endNotReached && insideRoads);
		}

    /*
    Debug.Log("útvonal DISTINCT a ciklus után :\n " + String.Join("",
							new List<GameObject>(calculatedRoute)
							.ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(), "\n"))
							.ToArray()));
     */

		this.gameObject.GetComponent<TweenHelper>().test = calculatedRoute.ToArray();
	}

  public List<GameObject> GetGameObjectsInCross(GameObject from)
  {
    List<GameObject> crossItems = new List<GameObject>();
    int indexEntry = calculatedRoute.IndexOf(from);

    GameObject temp = calculatedRoute[indexEntry];
    crossItems.Add(calculatedRoute[indexEntry]);

    do
    {
      indexEntry++;
      temp = calculatedRoute[indexEntry];
      crossItems.Add(calculatedRoute[indexEntry]);
      

    } while (temp.tag != "CrossRoad");

    /*
    Debug.Log("ezek az elemek az adott kereszteződésben :\n " + String.Join("",
              new List<GameObject>(crossItems)
              .ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(), "\n"))
              .ToArray()));
    */
    return crossItems;
  }
}
