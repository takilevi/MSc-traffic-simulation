using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DiscoverNeighbours : MonoBehaviour
{
	public Dictionary<GameObject, GameObject> carsToDestinations;
	public GameObject discoverStartObj;
	public bool manualTriggerRediscover = false;

	public GameObject[] allRoadGameObject;
	public GameObject[] allCrossRoadGameObject;
	public CrossRoadMeta[] crossMetaObject;
	List<GameObject> allRoad;

	public List<GameObject> calculatedRoute;
	public GameObject from;
	public GameObject to;

	void Awake()
	{
		allRoadGameObject = GameObject.FindGameObjectsWithTag("RoadModel");
		allCrossRoadGameObject = GameObject.FindGameObjectsWithTag("CrossRoad");
		allRoad = new List<GameObject>();
		allRoad.AddRange(allRoadGameObject);
		allRoad.AddRange(allCrossRoadGameObject);

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

		foreach (var item in crossMetaObject)
		{
			//A* -hoz szükséges számítások
			//csúcspontok súlyozása G-H-F értékek
			item.CallGCalculationOnFromElements();
			item.CallHCalculationOnExitElements(to);
		}

		//innen kezdődik maga az algoritmus

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
	}
}
