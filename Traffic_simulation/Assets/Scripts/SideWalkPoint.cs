using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWalkPoint : PathPoint {


	// Use this for initialization
	void Awake () {
    thisColor = Color.yellow;
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnDrawGizmos()
  {
    Gizmos.color = thisColor;
    Gizmos.DrawWireSphere(transform.position, .20f);
  }
}
