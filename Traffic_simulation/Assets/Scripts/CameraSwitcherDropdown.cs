using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcherDropdown : MonoBehaviour
{
  Dropdown mDropdown;
  int mDropdownValue = 0;
  List<GameObject> gameCameras;

  void Start()
  {
    mDropdown = GetComponent<Dropdown>();

    mDropdown.onValueChanged.AddListener(delegate {
      DropdownValueChanged(mDropdown);
    });

    gameCameras = new List<GameObject>();

    gameCameras.Add(GameObject.FindGameObjectWithTag("MainCamera"));
    gameCameras.AddRange(GameObject.FindGameObjectsWithTag("CustomCamera"));

    foreach (var item in gameCameras)
    {
      mDropdown.options.Add(new Dropdown.OptionData(item.name));
    }
    
    Debug.Log("Starting Dropdown Value : " + mDropdown.value);
  }

  private void DropdownValueChanged(Dropdown change)
  {
    foreach (GameObject item in gameCameras)
    {
      if (gameCameras.IndexOf(item) == change.value)
      {
        item.SetActive(true);
      }
      else
      {
        item.SetActive(false);
      }
    }
  }

  void Update()
  {
  }

  
}
