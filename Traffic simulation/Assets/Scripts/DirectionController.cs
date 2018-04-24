using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionController : MonoBehaviour {

  public Transform[] path;
  public Transform[] rightDirectionPath;
  public Transform[] forwardDirectionPath;
  public Transform[] leftDirectionPath;
  private List<Transform[]> directionList = new List<Transform[]>();
  public GameObject mainRoadTrafficLight;

  public enum TrafficLight {Red,Yellow,Green};
  public TrafficLight mainRoadLight = TrafficLight.Red;
  private TrafficLight activeLight;
  public bool buttonActivated = false;
  private bool previousCheck;

  void Start()
  {
    directionList.Add(rightDirectionPath);
    directionList.Add(forwardDirectionPath);
    directionList.Add(leftDirectionPath);
    activeLight = mainRoadLight;

    previousCheck = buttonActivated;

    GetStarted("init");
  }

  void Update()
  {
    if (buttonActivated)
    {
      iTween.Pause(gameObject, true);
      previousCheck = buttonActivated;
    }
    if (previousCheck!=buttonActivated)
    {
      iTween.Resume(gameObject, true);
    }

    if(activeLight != mainRoadLight)
    {
      mainRoadTrafficLight.GetComponent<TrafficLightController>().SwitchTo(mainRoadLight);
      activeLight = mainRoadLight;
    }
  }

  void OnGUI()
  {
    
  }

  void OnDrawGizmos()
  {
    //iTween.DrawPath(path);
    //iTween.DrawPath(rightDirectionPath);
    //iTween.DrawPath(forwardDirectionPath);
    //iTween.DrawPath(leftDirectionPath);
  }

  void GetStarted(string direction)
  {
    switch (direction)
    {
      case "init":
      iTween.MoveTo(gameObject,
      iTween.Hash("path", path, "time", 10, "orienttopath", true,
      "looktime", .6, "easetype", iTween.EaseType.linear , "onComplete", "ChooseDirection"));
        break;

        /*iTween.MoveTo(gameObject,
          iTween.Hash("path", path, "time", 10, "orienttopath", true,
          "looktime", .6, "easetype", "easeInOutSine", "onComplete", "ChooseDirection", "onCompleteParams", "left"));*/

    }

  }

  void ChooseDirection()
  {
    iTween.MoveTo(gameObject,
      iTween.Hash("path", forwardDirectionPath, "time", 5, "orienttopath", true,
      "looktime", .6, "easetype", iTween.EaseType.linear));

    //StartCoroutine(AlwaysWaitAtStopSign(Random.Range(0,directionList.Count)));

    //TODO: chose from the available paths
  }

  IEnumerator AlwaysWaitAtStopSign(int pathSelector)
  {
    Debug.Log("választott irány: "+pathSelector);
    //yield return new WaitForSeconds(2f);

    yield return new WaitUntil(() => mainRoadLight == TrafficLight.Green);
    Debug.Log("vártam");
    iTween.MoveTo(gameObject,
      iTween.Hash("path", directionList[pathSelector], "time", 5, "orienttopath", true,
      "looktime", .6, "easetype", "easeInOutSine", "onComplete", "GetStarted", "onCompleteParams", "init"));
  }

  void CheckRules()
  {
    //TODO: check the rules of the desired direction
  }

  void PathSelection()
  {
    //early predicted method
  }


  void MoveCamera()
  {
    iTween.MoveUpdate(Camera.main.gameObject,
      new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 30, this.gameObject.transform.position.z + 10f), 1f);
  }
}
