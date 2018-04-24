using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrafficLightController : MonoBehaviour {

  public GameObject redLight;
  public GameObject yellowLight;
  public GameObject greenLight;
  // Use this for initialization
  void Start() {
    redLight.SetActive(true);
  }

  // Update is called once per frame
  void Update() {

  }

  public void SwitchTo(DirectionController.TrafficLight light)
  {
    redLight.SetActive(false); yellowLight.SetActive(false); greenLight.SetActive(false);
    switch (light)
    {
      case DirectionController.TrafficLight.Red:
        redLight.SetActive(true);
        break;
      case DirectionController.TrafficLight.Yellow:
        yellowLight.SetActive(true);
        break;
      case DirectionController.TrafficLight.Green:
        greenLight.SetActive(true);
        break;

    }
  }
}
