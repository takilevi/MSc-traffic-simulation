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
[DiscoverNeighbours.cs](https://github.com/takilevi/MSc-traffic-simulation/blob/Msc-onlab-2/Traffic_simulation/Assets/Scripts/DiscoverNeighbours.cs) has almost all of the responsibilities from the pathfinding process.

The **FindShortestPath** method creates the path, which has two parameters: _from_ and _to_ GameObjects.

Here are some documentations about the steps of the A* here:

* https://www.raywenderlich.com/3016-introduction-to-a-pathfinding
* https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
* https://en.wikipedia.org/wiki/A*_search_algorithm

