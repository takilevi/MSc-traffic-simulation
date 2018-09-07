using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoadModel : PathPoint
{
	public GameObject closestRoad;

	public GameObject ClosestRoad
	{
		get { return closestRoad; }
		set { closestRoad = value; }
	}

	public float G = float.PositiveInfinity;
	public float H = float.PositiveInfinity;
	public float F = float.PositiveInfinity;
	public GameObject canConnectToFromExit;

	// Use this for initialization
	void Awake()
	{
		thisColor = Color.cyan;
	}

	// Update is called once per frame
	void Update()
	{
		F = G + H;
	}

	override
	public void FindMyNeighbours()
	{
		CrossRoadMeta meta = this.GetComponentInParent<CrossRoadMeta>();
		List<GameObject> options = new List<GameObject>();
		bool isFrom = false;

		foreach (var item in meta.roadGroup)
		{
			if (GameObject.ReferenceEquals(this.gameObject, item.from))
			{
				isFrom = true;
				options = item.options;
			}
		}

		if (isFrom)
		{
			foreach (var item in options)
			{
				//Debug.Log("crossroadban vagyok, ez egy from elem; szülő: " + this.transform.parent);

				if (item.GetComponent<CrossRoadModel>().ClosestRoad == null)
				{
					GameObject myNeighbourForward = ElementTable.MyClosestNeighbour(item, item.transform.forward * 5);
					item.GetComponent<CrossRoadModel>().ClosestRoad = myNeighbourForward;

					RoadElementModel component = myNeighbourForward.GetComponent<RoadElementModel>();

					if (component.PreviousElement == null)
					{
						component.PreviousElement = item;
					}
					component.FindMyNeighbours();

				}
			}
		}
	}

	public void CalculateGValue()
	{
		GameObject prev = closestRoad;
		G = 0;
		while (prev.GetComponent<RoadElementModel>() != null)
		{
			G++;

			prev = prev.GetComponent<RoadElementModel>().previousElement;
		}
		//Debug.Log("ennyi útelem van mögöttem : " + G + " ------- " + gameObject.name + " -------- " + transform.parent.name);
		prev.GetComponent<CrossRoadModel>().canConnectToFromExit = this.gameObject;
	}

	public void CalculateHValue( GameObject to)
	{
		//Egy ilyen csempe 5 hosszú ezért osztok öttel hogy a G-vel ugyanolyan súlyúak legyenek, így lesz valid a számolás
		H = Vector3.Distance(transform.position, to.transform.position) / 5;
		//Debug.Log(transform.position + " ---- " + to.transform.position + " ---- " + H + " ---- " + transform.parent.name);
	}
}
