using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
  /* source: https://gist.github.com/gunderson/d7f096bd07874f31671306318019d996
    
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


  float mainSpeed = 100.0f; //regular speed
  float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
  float maxShift = 1000.0f; //Maximum speed when holdin gshift
  float camSens = 0.10f; //How sensitive it with mouse
  private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
  private float totalRun = 1.0f;
  bool dragged = false;

  void Update()
  {

    lastMouse = Input.mousePosition - lastMouse;
    lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
    lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
    if (Input.GetMouseButtonDown(1))
    {
      dragged = true;
    }
    if (dragged)
    {
      transform.eulerAngles = lastMouse;
      if (Input.GetMouseButtonUp(1))
      {
        dragged = false;
      }
    }

    lastMouse = Input.mousePosition;
    //Mouse  camera angle done.  

    //Keyboard commands
    Vector3 p = GetBaseInput();
    if (Input.GetKey(KeyCode.LeftShift))
    {
      totalRun += Time.deltaTime;
      p = p * totalRun * shiftAdd;
      p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
      p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
      p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
    }
    else
    {
      totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
      p = p * mainSpeed;
    }

    p = p * Time.deltaTime;
    Vector3 newPosition = transform.position;
    if (!Input.GetKey(KeyCode.Space))
    { //If player wants to move on X and Z axis only
      transform.Translate(p);
      newPosition.x = transform.position.x;
      newPosition.z = transform.position.z;
      transform.position = newPosition;
    }
    else
    {
      transform.Translate(p);
    }

    float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");           //This little peece of code is written by JelleWho https://github.com/jellewie
    if (ScrollWheelChange != 0)
    {                                            //If the scrollwheel has changed
      float R = ScrollWheelChange * 15;                                   //The radius from current camera
      float PosX = this.transform.eulerAngles.x + 90;              //Get up and down
      float PosY = -1 * (this.transform.eulerAngles.y - 90);       //Get left to right
      PosX = PosX / 180 * Mathf.PI;                                       //Convert from degrees to radians
      PosY = PosY / 180 * Mathf.PI;                                       //^
      float X = R * Mathf.Sin(PosX) * Mathf.Cos(PosY);                    //Calculate new coords
      float Z = R * Mathf.Sin(PosX) * Mathf.Sin(PosY);                    //^
      float Y = R * Mathf.Cos(PosX);                                      //^
      float CamX = this.transform.position.x;                      //Get current camera postition for the offset
      float CamY = this.transform.position.y;                      //^
      float CamZ = this.transform.position.z;                      //^
      this.transform.position = new Vector3(CamX + X, CamY + Y, CamZ + Z);//Move the main camera
    }

  }

  private Vector3 GetBaseInput()
  { //returns the basic values, if it's 0 than it's not active.
    Vector3 p_Velocity = new Vector3();
    if (Input.GetKey(KeyCode.W))
    {
      p_Velocity += new Vector3(0, 0, 1);
    }
    if (Input.GetKey(KeyCode.S))
    {
      p_Velocity += new Vector3(0, 0, -1);
    }
    if (Input.GetKey(KeyCode.A))
    {
      p_Velocity += new Vector3(-1, 0, 0);
    }
    if (Input.GetKey(KeyCode.D))
    {
      p_Velocity += new Vector3(1, 0, 0);
    }
    return p_Velocity;
  }

}
