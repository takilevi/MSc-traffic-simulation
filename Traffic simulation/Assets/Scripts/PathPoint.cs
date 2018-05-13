using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PathPoint : MonoBehaviour {
  public Vector3 Position { get; set; }

  void Awake()
  {
    Position = transform.position;

  }

  void OnDrawGizmos(){
		Gizmos.color=Color.blue;
		Gizmos.DrawWireSphere(Position,.25f);
	}
}
