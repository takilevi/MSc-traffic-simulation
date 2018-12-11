using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CarController : MonoBehaviour
{
  public GameObject safetyCar = null;
  public bool carInCross = false;
  public bool carInWaitRow = false;

  private void Start()
  {
    carInCross = false;
    carInWaitRow = false;
  }

  private void Update()
  {
    if (safetyCar != null &&
      !Mathf.Approximately(this.GetComponent<TweenHelper>().currentSpeed, safetyCar.GetComponent<TweenHelper>().currentSpeed) &&
      this.GetComponent<TweenHelper>().speed > safetyCar.GetComponent<TweenHelper>().currentSpeed)
    {
      this.gameObject.GetComponent<TweenHelper>().SetCurrentSpeed(safetyCar.GetComponent<TweenHelper>().currentSpeed);
    }
    if (safetyCar!=null && safetyCar.GetComponent<CarController>().carInCross)
    {
      safetyCar = null;
      this.gameObject.GetComponent<TweenHelper>().RestoreToInitialSpeed();
    }

    if(!carInWaitRow && safetyCar == null && this.gameObject.GetComponent<TweenHelper>().currentSpeed != this.gameObject.GetComponent<TweenHelper>().speed)
    {
      this.gameObject.GetComponent<TweenHelper>().RestoreToInitialSpeed();
    }
  }


  private void OnTriggerEnter(Collider other)
  {

    if (other.gameObject.tag.Equals("Car") && ThisIsSafetyCar(other.gameObject))
    {
      if (!other.gameObject.GetComponent<CarController>().carInCross)
      {
        this.gameObject.GetComponent<TweenHelper>().SetCurrentSpeed(other.gameObject.GetComponent<TweenHelper>().currentSpeed);
        safetyCar = other.gameObject;
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {

    if (other.gameObject.tag.Equals("Car") && GameObject.ReferenceEquals(safetyCar, other.gameObject))
    {
      safetyCar = null;
      this.gameObject.GetComponent<TweenHelper>().RestoreToInitialSpeed();
    }
  }

  public bool ThisIsSafetyCar(GameObject other)
  {
    bool safetyCar = false;

    Vector3 myPosi = this.gameObject.transform.position;
    Vector3 otherPosi = other.transform.position;

    var heading = otherPosi - myPosi;

    var distance = heading.magnitude;
    var direction = heading / distance;

    safetyCar = V3Equal(direction,this.gameObject.transform.forward);

    if (safetyCar)
    {
      //Debug.Log(string.Format("ütközött velem ({0}) ez az objektum: {1} , a kiszámított direction: {2}, előtte van a safetycar : {3}", this.name, other.name, direction, safetyCar));
    }
    else if(Vector3.Distance(direction, this.gameObject.transform.forward) != 2f)
    {
      //Debug.Log(string.Format("nem adta ({0}) ez az objektum: {1} , a kiszámított távolság: {2}", this.name, other.name, Vector3.Distance(direction, this.gameObject.transform.forward)));
    }
    return safetyCar;
  }

  public bool V3Equal(Vector3 a, Vector3 b)
  {
    return Vector3.SqrMagnitude(a - b) < 0.15;
  }
}
