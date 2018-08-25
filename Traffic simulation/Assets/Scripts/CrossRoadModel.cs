using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoadModel : PathPoint
{
  private GameObject closestRoad;

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
}
