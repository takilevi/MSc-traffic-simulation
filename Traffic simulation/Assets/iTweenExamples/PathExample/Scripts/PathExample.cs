using UnityEngine;
using System.Collections;

public class PathExample : MonoBehaviour{
	public Transform[] path;
	public bool buttonActivated;
	
	void Start(){
		tween();
	}

  void Update()
  {
    //MoveCamera();
  }
	
	void OnGUI () {
		if(buttonActivated){
				reset();
				tween();
		}
	}
	
	void OnDrawGizmos(){
		iTween.DrawPath(path);
	}
	
	void tween(){
		iTween.MoveTo(gameObject,
      iTween.Hash("path",path,"time",30,"orienttopath",true,
      "looktime",.6, "easetype", "easeInOutSine", "looptype",iTween.LoopType.loop));
  }

  void reset(){
		buttonActivated=false;
		transform.position=new Vector3(0,0,0);
		transform.eulerAngles=new Vector3(0,0,0);
	}
	

  void MoveCamera()
  {
    iTween.MoveUpdate(Camera.main.gameObject, 
      new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 30, this.gameObject.transform.position.z + 10f), 1f);
  }
}

