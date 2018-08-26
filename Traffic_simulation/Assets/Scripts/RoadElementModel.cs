using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadElementModel : PathPoint {

  public GameObject previousElement;

  public GameObject PreviousElement
  {
    get { return previousElement; }
    set { previousElement = value; }
  }

  public GameObject nextElement;
  public GameObject NextElement
  {
    get { return nextElement; }
    set { nextElement = value; }
  }

  void Start () {
		
	}

  // Update is called once per frame
  void Update () {

  }

  override
  public void FindMyNeighbours()
  {
    if(nextElement == null)
    {
      GameObject myNeighbourForward = ElementTable.MyClosestNeighbour(this.gameObject, this.transform.forward * 5);

      NextElement = myNeighbourForward;

      if (myNeighbourForward.tag.Equals("RoadModel"))
      {
        RoadElementModel component = myNeighbourForward.GetComponent<RoadElementModel>();

        if(component.PreviousElement == null)
        {
          component.PreviousElement = this.gameObject;
        }
        component.FindMyNeighbours();

      }

      if (myNeighbourForward.tag.Equals("CrossRoad"))
      {
        CrossRoadModel component = myNeighbourForward.GetComponent<CrossRoadModel>();

        if(component.ClosestRoad == null)
        {
          component.ClosestRoad = this.gameObject;
        }

        component.FindMyNeighbours();

      }
      
    }

    if (previousElement == null)
    {
      GameObject myNeighbourBackward = ElementTable.MyClosestNeighbour(this.gameObject, (-1) * this.transform.forward * 5);

      PreviousElement = myNeighbourBackward;

      if (myNeighbourBackward.tag.Equals("RoadModel"))
      {
        RoadElementModel component = myNeighbourBackward.GetComponent<RoadElementModel>();

        if (component.NextElement == null)
        {
          component.NextElement = this.gameObject;
        }
        component.FindMyNeighbours();

      }

      if (myNeighbourBackward.tag.Equals("CrossRoad"))
      {
        CrossRoadModel component = myNeighbourBackward.GetComponent<CrossRoadModel>();

        if (component.ClosestRoad == null)
        {
          component.ClosestRoad = this.gameObject;
        }

        component.FindMyNeighbours();

      }
    }
  }
}
