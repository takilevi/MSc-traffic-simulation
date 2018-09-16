using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadElementModel : PathPoint {

  public GameObject previousElement;
  private Color startcolor;

  public GameObject PreviousElement
  {
    get { return previousElement; }
    set { previousElement = value; }
  }

  public GameObject nextElement;
  public GameObject NextElement
  {
    get { return nextElement; }
    set { nextElement = value; }
  }

  private Color highlightColor = Color.green;
  private Renderer ownRenderer = null;

  public Color[] originalColors;
  private void Start()
  {
    if (ownRenderer == null) { ownRenderer = GetComponentInChildren<Renderer>(); }
    StoreOriginalColor();
  }
  // Update is called once per frame
  void Update()
  {

  }
  private void StoreOriginalColor()
  {
    if (ownRenderer != null)
    {
      Material[] materials = ownRenderer.materials;
      originalColors = new Color[materials.Length];
      for (int i = 0; i < materials.Length; ++i) { originalColors[i] = materials[i].color; }
    }
  }
  private void OnMouseEnter()
  {
    Debug.Log("Detected" + ownRenderer);
    if (ownRenderer != null)
    {
      Material[] materials = ownRenderer.materials;
      for (int i = 0; i < materials.Length; ++i) { materials[i].color = highlightColor; }
      ownRenderer.materials = materials;
    }
  }
  private void OnMouseExit()
  {
    Debug.Log("UNDetected" + ownRenderer);
    if (ownRenderer != null)
    {
      Material[] materials = ownRenderer.materials;
      for (int i = 0; i < materials.Length; ++i) { materials[i].color = originalColors[i]; }
      ownRenderer.materials = materials;
    }
  }



  override
  public void FindMyNeighbours()
  {
    if(nextElement == null)
    {
      GameObject myNeighbourForward = ElementTable.MyClosestNeighbour(this.gameObject, this.transform.forward * 5);

      NextElement = myNeighbourForward;

      if (myNeighbourForward.tag.Equals("RoadModel"))
      {
        RoadElementModel component = myNeighbourForward.GetComponent<RoadElementModel>();

        if(component.PreviousElement == null)
        {
          component.PreviousElement = this.gameObject;
        }
        component.FindMyNeighbours();

      }

      if (myNeighbourForward.tag.Equals("CrossRoad"))
      {
        CrossRoadModel component = myNeighbourForward.GetComponent<CrossRoadModel>();

        if(component.ClosestRoad == null)
        {
          component.ClosestRoad = this.gameObject;
        }

        component.FindMyNeighbours();

      }
      
    }

    if (previousElement == null)
    {
      GameObject myNeighbourBackward = ElementTable.MyClosestNeighbour(this.gameObject, (-1) * this.transform.forward * 5);

      PreviousElement = myNeighbourBackward;

      if (myNeighbourBackward.tag.Equals("RoadModel"))
      {
        RoadElementModel component = myNeighbourBackward.GetComponent<RoadElementModel>();

        if (component.NextElement == null)
        {
          component.NextElement = this.gameObject;
        }
        component.FindMyNeighbours();

      }

      if (myNeighbourBackward.tag.Equals("CrossRoad"))
      {
        CrossRoadModel component = myNeighbourBackward.GetComponent<CrossRoadModel>();

        if (component.ClosestRoad == null)
        {
          component.ClosestRoad = this.gameObject;
        }

        component.FindMyNeighbours();

      }
    }
  }
}
