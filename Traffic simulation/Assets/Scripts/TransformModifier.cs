using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransformModifier : PathPoint {

  //public Color thisColor = Color.yellow;

  void Awake () {
    Position = this.transform.position - this.transform.right * 2.5f - this.transform.forward * 2.5f;
  }
	
	void Update () {
		
	}

  private void OnDrawGizmos()
  {
    Gizmos.color = thisColor;
    Gizmos.DrawWireSphere(Position, .20f);
  }
  public void SetColor(Color color)
  {
    thisColor = color;
  }
}
