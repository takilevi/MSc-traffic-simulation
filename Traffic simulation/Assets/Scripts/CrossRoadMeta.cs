using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoadMeta : MonoBehaviour {

  [System.Serializable]
  public class RoadGroup
  {
    public GameObject from;
    public List<GameObject> options;
  }
  public RoadGroup[] roadGroup;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
