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

	[System.Serializable]
	public class AStarCalc
	{
		public GameObject from;
		public float G = float.PositiveInfinity;
		public float H = float.PositiveInfinity;
		public float F = float.PositiveInfinity;

		public AStarCalc(GameObject from, float g, float h, float f)
		{
			this.from = from;
			G = g;
			H = h;
			F = f;
		}
	}
	public List<AStarCalc> aStarValues = new List<AStarCalc>();


	public GameObject canConnectToFromExit;

	// Use this for initialization
	void Awake()
	{
		thisColor = Color.cyan;
	}

	// Update is called once per frame
	void Update()
	{
		foreach (var item in aStarValues)
		{
			item.F = item.G + item.H;
		}
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

	public void CalculateGValue( GameObject from )
	{
		GameObject prev = closestRoad;
		AStarCalc contains = null;
		foreach (var item in aStarValues)
		{
			if(GameObject.ReferenceEquals(item.from, from))
			{
				contains = item;
			}
		}
		if(contains == null)
		{
			contains = new AStarCalc(from, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			aStarValues.Add(contains);
		}

		contains.G = 0;
		while (prev.GetComponent<RoadElementModel>() != null)
		{
			contains.G++;

			prev = prev.GetComponent<RoadElementModel>().previousElement;
		}
		//Debug.Log("ennyi útelem van mögöttem : " + G + " ------- " + gameObject.name + " -------- " + transform.parent.name);
		prev.GetComponent<CrossRoadModel>().canConnectToFromExit = this.gameObject;
	}

	public void CalculateHValue( GameObject from, GameObject to)
	{
		AStarCalc contains = null;
		foreach (var item in aStarValues)
		{
			if (GameObject.ReferenceEquals(item.from, from))
			{
				contains = item;
			}
		}
		if (contains == null)
		{
			contains = new AStarCalc(from, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			aStarValues.Add(contains);
		}

		//Egy ilyen csempe 5 hosszú ezért osztok öttel hogy a G-vel ugyanolyan súlyúak legyenek, így lesz valid a számolás

		contains.H = Vector3.Distance(transform.position, to.transform.position) / 5;
		//Debug.Log(transform.position + " ---- " + to.transform.position + " ---- " + H + " ---- " + transform.parent.name);
	}

	public float GetH( GameObject from)
	{
		foreach (var item in aStarValues)
		{
			if (GameObject.ReferenceEquals(item.from, from))
			{
				return item.H;
			}
		}
		return float.PositiveInfinity;
	}

	public float GetG(GameObject from)
	{
		foreach (var item in aStarValues)
		{
			if (GameObject.ReferenceEquals(item.from, from))
			{
				return item.G;
			}
		}
		return float.PositiveInfinity;
	}
}
