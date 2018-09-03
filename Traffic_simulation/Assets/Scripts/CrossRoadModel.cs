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

  // Use this for initialization
  void Awake () {
    thisColor = Color.cyan;
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  override
  public void FindMyNeighbours()
  {
    CrossRoadMeta meta = this.GetComponentInParent<CrossRoadMeta>();
    List<GameObject> options = new List<GameObject>();
    bool isFrom = false;

    foreach (var item in meta.roadGroup)
    {
      if(GameObject.ReferenceEquals(this.gameObject, item.from))
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

        if(item.GetComponent<CrossRoadModel>().ClosestRoad == null)
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
}
