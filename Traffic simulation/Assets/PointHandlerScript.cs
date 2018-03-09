using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHandlerScript : MonoBehaviour {
  public GameObject[] points;

	// Use this for initialization
	void Start () {
    AddBasicPathPointScript(points);

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void AddBasicPathPointScript(GameObject[] points)
  {
    foreach (var item in points)
    {
      item.AddComponent<PathPoint>();
    }
  }
}
