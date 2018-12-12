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
##### The main idea
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


---

## Simulation

The program can handle up to 200 cars, without performance issues. 
However the city can barely handle over 40-50 cars, because of the main crossroads. At one point they block themselves to a deadlock.


