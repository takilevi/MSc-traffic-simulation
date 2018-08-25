using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadElementModel : PathPoint {

  private GameObject previousElement;

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
}
