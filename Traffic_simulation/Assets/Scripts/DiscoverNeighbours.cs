using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DiscoverNeighbours : MonoBehaviour
{
	public GameObject discoverStartObj;

	public GameObject[] allRoadGameObject;
	public GameObject[] allCrossRoadGameObject;
	public CrossRoadMeta[] crossMetaObject;
	List<GameObject> allRoad;

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

	void Awake()
	{
		allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadModel");
		allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
		allRoad = new List<GameObject>();
		allRoad.AddRange(allRoadGameObject);
		allRoad.AddRange(allCrossRoadGameObject);

		openList = new Queue<AStarNode>();
		closedList = new Queue<AStarNode>();

		crossMetaObject = GameObject.FindObjectsOfType<CrossRoadMeta>();

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
		FindShortestPath(from, to);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void StartDiscover(GameObject startObj)
	{
		foreach (Transform child in startObj.transform)
		{
			child.gameObject.GetComponent<RoadElementModel>().FindMyNeighbours();
		}
	}

	void FindShortestPath(GameObject from, GameObject to)
	{
		foreach (var item in crossMetaObject)
		{
			//A* -hoz szükséges számítások
			//csúcspontok súlyozása G-H-F értékek
			item.CallGCalculationOnFromElements();
			item.CallHCalculationOnExitElements(to);
		}

		calculatedRoute.Add(from);

		bool straightRoad = true;
		List<GameObject> exitsFromStart = new List<GameObject>();
		while (straightRoad)
		{
			GameObject nextElement = calculatedRoute[calculatedRoute.Count - 1].GetComponent<RoadElementModel>().nextElement;

			if (GameObject.ReferenceEquals(nextElement, to))
			{
				Debug.Log("megvagyunk");
				return;
			}
			Component nextComp = nextElement.GetComponent<CrossRoadModel>();
			if (nextComp != null)
			{
				exitsFromStart = nextComp.GetComponentInParent<CrossRoadMeta>().GetOptions(nextElement);
				straightRoad = false;

				foreach (var item in exitsFromStart)
				{
					openList.Enqueue(new AStarNode(item, null, calculatedRoute.Count + item.GetComponent<CrossRoadModel>().H));
					//openList.Sort((x, y) => x.F.CompareTo(y.F));
					openList = new Queue<AStarNode>(openList.OrderBy(x => x.F));
				}

			}
			calculatedRoute.Add(nextElement);

		}

		//ha megvan az elem akkor már kiléptünk
		//ha megvan a legközelebbi kereszteződés akkor ide jutottunk, a lehetséges kimenetelek a exits változóban

		bool searchNearestCrossToEnd = true;
		List<GameObject> entrancesToEnd = new List<GameObject>();
		GameObject prevUntilClosestCross = to;
		while (searchNearestCrossToEnd)
		{
			GameObject prevElement = prevUntilClosestCross.GetComponent<RoadElementModel>().previousElement;

			Component prevComp = prevElement.GetComponent<CrossRoadModel>();
			if (prevComp != null)
			{
				entrancesToEnd = prevComp.GetComponentInParent<CrossRoadMeta>().PossibleEntrancesToExit(prevElement);
				searchNearestCrossToEnd = false;
			}

			prevUntilClosestCross = prevElement;
		}

		/*
		Debug.Log("Calculated route :\n " + String.Join("",
						 new List<GameObject>(calculatedRoute)
						 .ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(),"\n"))
						 .ToArray()));
		*/


		//megvannak a CÉL legközelebbi entrance pontjai
		//az exitsFromStart-ból az egyik entranceToEnd-ig kell eljutni
		//egy gyors forciklusban megnézni hogy az exitek és az egyetlen bejárat közt van e átfedés - magyarul ugyanabba a kereszteződésbe érkezünk-e

		if (exitsFromStart.Contains(prevUntilClosestCross))
		{
			Debug.Log("megvagyunk");
			Debug.Log(prevUntilClosestCross.name + "......közös be/kijárat....." + prevUntilClosestCross.transform.parent.name);
			return;
		}

		//még egy kics shortcut ellenőrzés

		List<GameObject> exitDetection = new List<GameObject>();
		foreach (var item in exitsFromStart)
		{
			exitDetection.Add(item.GetComponent<CrossRoadModel>().canConnectToFromExit);
		}
		Debug.Log("Entrance to end object :\n " + String.Join("",
						 new List<GameObject>(entrancesToEnd)
						 .ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(), "\n"))
						 .ToArray()));
		Debug.Log("Exit connections from start object :\n " + String.Join("",
						 new List<GameObject>(exitDetection)
						 .ConvertAll(i => String.Concat(i.ToString(), i.transform.parent.ToString(), "\n"))
						 .ToArray()));

		if (entrancesToEnd.Intersect(exitDetection).Any())
		{
			Debug.Log("megvagyunk");
		}

		//innen kezdődik maga az algoritmus

		Debug.Log("Open list :\n " + String.Join("",
						 new List<AStarNode>(openList)
						 .ConvertAll(i => String.Concat(i.node.ToString(), i.node.transform.parent.ToString(), "\t", i.F, "\n"))
						 .ToArray()));


		do
		{
			closedList.Enqueue(openList.Dequeue());
			if (GameObject.ReferenceEquals(prevUntilClosestCross,closedList.ToList().Last().node))
			{
				//path found
				//DONE
				Debug.Log("DONE");
				AStarNode arrive = closedList.ToList().Last();
				AStarNode nextReverse = arrive;
				do
				{
					Debug.Log("parent node: " + nextReverse.node + " ----- " + nextReverse.node.transform.parent);
					nextReverse = nextReverse.parent;

				} while (nextReverse != null);
				break;
			}

			GameObject adjacentEntrance = closedList.ToList().Last().node.GetComponent<CrossRoadModel>().canConnectToFromExit;
			List<GameObject> adjacentExits = adjacentEntrance.GetComponentInParent<CrossRoadMeta>().GetOptions(adjacentEntrance);
			List<AStarNode> adjacentAStarNodes = new List<AStarNode>();
			foreach (var item in adjacentExits)
			{
				adjacentAStarNodes.Add(new AStarNode(item, closedList.ToList().Last(), closedList.ToList().Last().F + adjacentEntrance.GetComponent<CrossRoadModel>().G + item.GetComponent<CrossRoadModel>().H));
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
					openList = new Queue<AStarNode>(openList.OrderBy(x => x.F));
				}
				else
				{
					//Recalculate the parent and the F value!!!
					
					//nemhiszem hogy ez a rész jó
					float oldF = oldOpen.F;
					float newF = aSquare.F;

					Debug.Log("oldF: " + oldF + " ------ newF: " + newF);
					Debug.Log("old node: " + oldOpen.node + " parent: " + oldOpen.node.transform.parent + " ------ new node: " + aSquare.node + " parent: " + aSquare.node.transform.parent);
					if (newF < oldF)
					{
						Debug.Log("bejöttünk a newF kisebb oldF ágba ---- oldparent : " + oldOpen.parent + "   --- newparent: " + closedList.ToList().Last().node);
						oldOpen.F = newF;
						oldOpen.parent = closedList.ToList().Last();
					}

					//not cool
				}
			}

			/*
			 *
       * https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
			 * 
			 * */

			Debug.Log("Closed list :\n " + String.Join("",
						new List<AStarNode>(closedList)
						.ConvertAll(i => String.Concat(i.node.ToString(), i.node.transform.parent.ToString(), "\t", i.F, "\n"))
						.ToArray()));

		} while (openList.Any());
	}
}
