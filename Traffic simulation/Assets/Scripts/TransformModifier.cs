using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformModifier : MonoBehaviour {

	// Use this for initialization
	void Start () {
    var pos = GetPosition();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public Vector3 GetPosition()
  {
    var rotation = Mathf.Round(this.gameObject.transform.rotation.eulerAngles.y);
    Debug.Log("forgás: "+ this.gameObject.transform.rotation.eulerAngles.y + "  név: " + this.gameObject.name);
    if (rotation == 0f)
    {
      Debug.Log("én 0 forgásszögű vagyok " + this.gameObject.name);
      return new Vector3(0, 0, 0);
    }
    if (rotation == 90f)
    {
      Debug.Log("én 90 forgásszögű vagyok " + this.gameObject.name);
      return new Vector3(0, 0, 0);
    }
    if (rotation == 270f)
    {
      Debug.Log("én -90 forgásszögű vagyok " + this.gameObject.name);
      return new Vector3(0, 0, 0);
    }
    if (rotation == 180f)
    {
      Debug.Log("én 180 forgásszögű vagyok " + this.gameObject.name);
      return new Vector3(0, 0, 0);
    }

    Debug.Log("kimaradtam "+rotation+" név:" + this.gameObject.name);
    return new Vector3(0,0,0);
  }
}
