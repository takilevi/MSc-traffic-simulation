using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadElementModel : TransformModifier {

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
    //Debug.DrawLine(this.Position, NextElement.GetComponent<TransformModifier>().Position, Color.red);
  }

  public void FindMyNextElementByDirection(Vector3 dir)
  {

  }
}
