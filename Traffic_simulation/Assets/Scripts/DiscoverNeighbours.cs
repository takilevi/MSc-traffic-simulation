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
		public GameObject parent;
		public float F;

		public AStarNode(GameObject node, GameObject parent, float f)
		{
			this.node = node;
			this.parent = parent;
			F = f;
		}
	}
	public List<AStarNode> openList;
	public List<AStarNode> closedList;

	void Awake()
	{
		allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadModel");
		allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
		allRoad = new List<GameObject>();
		allRoad.AddRange(allRoadGameObject);
		allRoad.AddRange(allCrossRoadGameObject);

		openList = new List<AStarNode>();
		closedList = new List<AStarNode>();

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
					openList.Add(new AStarNode(item, null, calculatedRoute.Count + item.GetComponent<CrossRoadModel>().H));
					openList.Sort((x, y) => x.F.CompareTo(y.F));
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
			/*
			 * 
			 * 
			 * 
			 	currentSquare = [openList squareWithLowestFScore]; // Get the square with the lowest F score
	
				[closedList add:currentSquare]; // add the current square to the closed list
					[openList remove:currentSquare]; // remove it to the open list
				
				if ([closedList contains:destinationSquare]) { // if we added the destination to the closed list, we've found a path
					// PATH FOUND
						break; // break the loop
				}
	
				adjacentSquares = [currentSquare walkableAdjacentSquares]; // Retrieve all its walkable adjacent squares
	
				foreach (aSquare in adjacentSquares) {
		
				if ([closedList contains:aSquare]) { // if this adjacent square is already in the closed list ignore it
					continue; // Go to the next adjacent square
				}
		
				if (![openList contains:aSquare]) { // if its not in the open list
			
					// compute its score, set the parent
					[openList add:aSquare]; // and add it to the open list
			
				} else { // if its already in the open list
			
					// test if using the current G score make the aSquare F score lower, if yes update the parent because it means its a better path
			
					}
				}

		https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
			 * 
			 * */
		} while (!openList.Any());
	}
}
