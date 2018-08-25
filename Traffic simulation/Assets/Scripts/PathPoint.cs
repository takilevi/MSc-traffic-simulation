using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PathPoint : MonoBehaviour {

  public Vector3 Position
  {
    get { return transform.position; }
    set { transform.position = value; }
  }

  public Color thisColor;

  private void Start()
  {
    
  }
  void Awake()
  {
    thisColor = Color.blue;

  }

  void OnDrawGizmos(){
		Gizmos.color= thisColor;
		Gizmos.DrawWireSphere(transform.position,.25f);
    Vector3 forward = transform.TransformDirection(Vector3.forward) * 2f;
    Debug.DrawRay(transform.position, forward, Color.green);
  }
}
