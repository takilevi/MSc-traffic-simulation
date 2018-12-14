# Traffic simulation
My MSc thesis preparation project - using _Unity3D_ engine - is about to simulate traffic in a virtual city.

The assets come from the [POLYGON - City Pack](https://assetstore.unity.com/packages/3d/environments/urban/polygon-city-pack-95214), with some modifications on the road elements and the cars.

## Road elements
Roads are built up from small tiles, each tile is a 5x5 element. 
In my model, streets have 2 lanes, so I made a prefab to encapsulate 2 tiles into one GameObject, which represent a 2-lane street tile.

![road](https://i.imgur.com/cso0iSh.png)
![road inspector](https://i.imgur.com/JNR57bb.png) 
![road encapsulation](https://i.imgur.com/m8y6eXC.png)


#### [RoadElementModel.cs](https://github.com/takilevi/MSc-traffic-simulation/blob/Msc-onlab-2/Traffic_simulation/Assets/Scripts/RoadElementModel.cs)
Stores information about the neighbour elements, and contains methods to discover the element's neighbours.

## Crossroads

Crossroads are different from the basic road elements. Has a lot additional logic and responsibilities.

The crossroads are the **nodes of the A\* algorithm**. While the basic road elements are the edges.

![3 types of crossroads](https://i.imgur.com/5DQs88L.png)
![crossroad meta](https://i.imgur.com/OBuA0ha.png)
![crossroad model](https://i.imgur.com/6S6p2zL.png)

There are 3 types of crossroads currently in my model. 2-3-and 4-arm crossroads. All of them can handle 2 lanes per arm. 

Each enter/exit tile element has a [CrossRoadModel.cs](https://github.com/takilevi/MSc-traffic-simulation/blob/Msc-onlab-2/Traffic_simulation/Assets/Scripts/CrossRoadModel.cs) script, 
and the parent object (encapsulating object) also has a script [CrossRoadMeta.cs](https://github.com/takilevi/MSc-traffic-simulation/blob/Msc-onlab-2/Traffic_simulation/Assets/Scripts/CrossRoadMeta.cs).

#### CrossRoadModel.cs
As the crossroad elements are the nodes of the A\* algorithm, the model script calculates the G,H and F values for the actual node.
The calculated values are in a list, because each car need a unique calculated value on the same crossing.

#### CrossRoadMeta.cs
Meta class stores the enter and the exit elements, and the relations between them. This meta class controls the entire crossroad, 
so the crossing management will happen in this script, such as lockings, and handle of the wait rows.

---
## A* pathfinding algorithm
[DiscoverNeighbours.cs](https://github.com/takilevi/MSc-traffic-simulation/blob/Msc-onlab-2/Traffic_simulation/Assets/Scripts/DiscoverNeighbours.cs) handles almost all of the responsibilities from the pathfinding process.

The **FindShortestPath** method creates the path, which has two parameters: _from_ and _to_ GameObjects.

Here are some documentations about the steps of the A* :

* https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
* https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
* https://en.wikipedia.org/wiki/A*_search_algorithm


As I mentioned before the crossroad elements are the nodes of the graph, and the basic road elements are the edges between the nodes.

**The main idea**

The most important thing about my implementation is the idea that **every road element is accessible from a crossroad**.

By that idea I can separate the pathfinding to 2 well defining groups:
1. Starting road element and the End road element are on the same 'edge', so no crossroad/node interrupt the path, this is the easy way, no need to calculate anything just chaining the road elements.
2. Starting road element and the End road element are not on the same edge, this means one or more crossroads/nodes interrupts the car's way from start to end.
I will write about this scenario in the next chapter 

### My implementation for A*

**Init:** Need a car GameObject with the DiscoverNeighbours script on it. And the script needs a 'from' and a 'to' road element to start the calculation.
And the crossroads will get their initial H and G values.

**First steps:** The first step is to find the closest crossroad elements from the stance points. For the staring point this means the closest entry point of a crossroad,
and for the endpoint this means the closest exit point of a crossroad.

**Iteration:** The script now checks the upon crossroad entry point and compares it to the desired exit point, and calculates the heuristic values.

**End of the algorithm:** The reached entry point of a crossroad is an element of the same crossroad as the desired exit point.

At the end of the pathfinding, we have all the related crossroads, with the specified entry and exit points.
Only the connection is missing, the connection itself is a chain of road elements, the script calculates it, between the crossroads, and voilá the calculated path is there.

---
## Crossroad locking

CrossRoadModel and CrossRoadMeta together handle the collision control on the crossroad elements.

If a car _arrives at an entry point_ it triggers an event. The car passes the direction where it want to go. Now two things can happen:
1. The passed elements are free on the crossroad, so the car can go further, and also locks those crossroad elements.
2. The elements (or at least one of the elements) are blocked, so the car has been added to a wait row.

If one car _passes the crossroad_ it also triggers an onExit type event. The car unlocks all the previously blocked elements behind its path.

![crossroad collider profile](https://i.imgur.com/rvfEDek.png)
![crossroad collider birdview](https://i.imgur.com/MLSeZhn.png)

As you can see the collision controlled by BoxColliders on each entry and exit nodes.

The BoxCollider is a component of the crosselement, so the _CrossRoadModel_ will handle its triggers, and the triggers delegated to the parent (_CrossRoadMeta_) which has methods called TriggerHandler and TriggerHandlerExit.

```
private void OnTriggerEnter(Collider other)
{
	this.GetComponentInParent<CrossRoadMeta>().TriggerHandler(other, this.gameObject);
}
private void OnTriggerExit(Collider other)
{
	this.GetComponentInParent<CrossRoadMeta>().TriggerHandlerExit(other, this.gameObject);
}
```

### Enter trigger

_TriggerHandler_ method handles the entry points. The implementation:

```
  public bool TriggerHandler(Collider other, GameObject road)
  {
    bool lockSuccess = false;
    if (RoadIsEntry(road))
    {
      List<GameObject> crossItems = other.gameObject.GetComponent<DiscoverNeighbours>().GetGameObjectsInCross(road);
      bool isCrossElementsLocked = CrossElementsLocked(crossItems);

      if (!isCrossElementsLocked)
      {
        //locking
        foreach (var crossItem in crossItems)
        {
          crossLock[crossItem] = other.gameObject;
        }
        if (Mathf.Approximately(other.gameObject.GetComponent<TweenHelper>().currentSpeed,0.0f))
        {
          other.gameObject.GetComponent<TweenHelper>().RestoreToInitialSpeed();
          other.gameObject.GetComponent<CarController>().carInWaitRow = false;
        }
        lockSuccess = true;
        other.gameObject.GetComponent<CarController>().carInCross = true;

      }

	  //at least one element of the crossroad chain is locked
      if (isCrossElementsLocked)
      {
        other.gameObject.GetComponent<TweenHelper>().SetCurrentSpeed(0.0f);
        other.gameObject.GetComponent<CarController>().carInWaitRow = true;
        if (!waitRow.ContainsKey(other))
        {
          waitRow.Add(other, road);
        }
      }
    }
    return lockSuccess;
  }
```

If the collided GameObject is an entry (_RoadIsEntry_ checks it), then the process starts.

_crossItems_ variable contains the crossroad elements from the calculated route by A*.

_isCrossElementsLocked_ is true, if all of the required elements of the collided car are free at the time.

1. Cross is free
	* Locks the required elements of the crossroad
	* Restore the speed to the initial one, if the car stopped before due to wait

2. Cross is locked
	* Set car's speed to zero (aka stop the car)
	* Put the car into the wait row
	

**crossLock** - _Key_: element of the cross  -  _Value_: locker car
```
public Dictionary<GameObject, GameObject> crossLock;
```

**waitRow** - _Key_: the collided entry point where the car waits  -  _Value_: waiting car
```
public Dictionary<Collider, GameObject> waitRow;
```

### Exit trigger

_TriggerHandlerExit_ method handles the exit points. The implementation:

```
  public void TriggerHandlerExit(Collider other, GameObject road)
  {
    List<GameObject> lockObj = new List<GameObject>();
    if (RoadIsExit(road))
    {
      //unlock where the 'other' is the locker
      foreach (var item in crossLock.Keys)
      {
        if (GameObject.ReferenceEquals(other.gameObject, crossLock[item]))
        {
          lockObj.Add(item);
        }
      }
      if(lockObj.Count != 0)
      {
        other.gameObject.GetComponent<CarController>().carInCross = false;
      }
      foreach (var item in lockObj)
      {
        crossLock[item] = null;
      }
    }

    if (lockObj.Count > 0)
    {
      CheckWaitRow();
    }
  }
```
If the collided GameObject is an exit point (_RoadIsExit_), then the release process begins.

Collect all of the locked objects by the car, and unlocks it, then call the _CheckWaitRow_ procedure.

_CheckWaitRow_ below:

```
  private void CheckWaitRow()
  {
    List<Collider> toRemoveList = new List<Collider>();

    if (waitRow.Count != 0)
    {
      foreach (var item in waitRow)
      {
        bool carCanGo = TriggerHandler(item.Key, item.Value);
        if(carCanGo)
        {
          toRemoveList.Add(item.Key);
        }
      }
    }

    foreach (var item in toRemoveList)
    {
      waitRow.Remove(item);
    }
  }
```
If the waitRow is not empty, then iterate through the list, and check if there is a car (or multiple cars) which can go on.

If the _carCanGo_ then it got removed from the waitRow, and place its locks as the _TriggerHandler_ does it.


## Collision prevention

As the program simulates a traffic, and the cars are fully autonome, I need to implement a prevention system.

![car colliders](https://i.imgur.com/A6WSXiC.png)

I use BoxColliders here too. The car GameObject has a component called [CarController.cs](https://github.com/takilevi/MSc-traffic-simulation/blob/Msc-onlab-2/Traffic_simulation/Assets/Scripts/CarController.cs) which handles the collisions.

```
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
```

If the car collides to another car, AND the _ThisIsSafetyCar_ returns with true. Then the _safetyCar_ variable setted to the collided object, and also sync their speeds.

When the _safetyCar_ leaves the BoxCollider's scope, then the car restores its speed to the initial one (the max speed). 

### Safety car

See the image ahead, if the white-orange Mustang reaches the small orange car, then the _safetyCar_ variable of the Mustang will be the small car GameObject.

The _ThisIsSafetyCar_ method checks that the collided car is ahead of the car - not behind of the car, and not a false detection, for example two cars' colliders can meet while both go through a crossroad.

## Simulation

The program can handle up to 200 cars, without performance issues. 
However the city can barely handle over 40-50 cars, because of the main crossroads. At one point they block themselves to a deadlock.


