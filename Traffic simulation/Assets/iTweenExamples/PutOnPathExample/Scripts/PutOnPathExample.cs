using UnityEngine;
using System.Collections;

public class PutOnPathExample : MonoBehaviour{
	public Transform[] path;
	public float percentage;
	
	void OnGUI () {
		GUILayout.Label ("Use slider to move object along path.");
		percentage=GUILayout.HorizontalSlider(percentage, 0, 1, GUILayout.Width (Screen.width));
		iTween.PutOnPath(gameObject,path,percentage);
		
		//You can cause the object to orient to its path by calculating a spot slightly ahead on the path for a look at target:
		transform.LookAt(iTween.PointOnPath(path,percentage+.05f));
	}
	
	void OnDrawGizmos(){
		iTween.DrawPath(path);
	}
}

