using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TweenHelper : MonoBehaviour
{
  public Transform[] test;
  public Vector3[] testV3;
  private Vector3[] pathPointsCatMull;
  public Color mainPathColor;

  public float speed = 250.0F;
  private GameObject targetObject;
  private int pathIndex = 1;
  private float reachDist = 1f;
  private bool moving;

  // Use this for initialization
  void Start()
  {
    targetObject = this.gameObject;
    targetObject.transform.position = test[0].position;

    pathPointsCatMull = GetInterpolatedPath(testV3).ToArray();
    //Debug.Log("hossz: " + pathPointsCatMull.Length);
    moving = true;
  }

  // Update is called once per frame
  void Update()
  {
    if (moving)
    {
      float dist = Vector3.Distance(pathPointsCatMull[pathIndex],transform.position);
      Debug.Log("speed: " + speed);
      transform.position = Vector3.Lerp(transform.position, pathPointsCatMull[pathIndex], Time.deltaTime*10*speed);
      transform.LookAt(pathPointsCatMull[pathIndex]);
      if (dist <= reachDist)
      { pathIndex++; }

      if (pathIndex >= pathPointsCatMull.Length)
      {
        moving = false;
      }
    }
  }
  private void OnDrawGizmos()
  {
    Vector3[] suppliedLine = new Vector3[test.Length];
    for (int i = 0; i < test.Length; i++)
    {
      suppliedLine[i] = test[i].position;
    }
    testV3 = suppliedLine;
    DrawForwardPath(testV3, mainPathColor);
  }

  private static void DrawForwardPath(Vector3[] path, Color color)
  {
    Vector3[] vector3s = PathControlPointGenerator(PathSmoothingAtCurvePoint(path));

    /*foreach (var item in vector3s)
    {
      Debug.Log(item);
    }*/

    //Line Draw:
    Vector3 previousPoint = Interp(vector3s, 0);
    Gizmos.color = color;
    int SmoothAmount = path.Length * 8;
    for (int i = 1; i <= SmoothAmount; i++)
    {
      float pm = (float)i / SmoothAmount;
      Vector3 currPt = Interp(vector3s, pm);
      Gizmos.DrawLine(currPt, previousPoint);
      previousPoint = currPt;
    }
  }

  private List<Vector3> GetInterpolatedPath(Vector3[] path)
  {
    Vector3[] vector3s = PathControlPointGenerator(PathSmoothingAtCurvePoint(path));

    List<Vector3> pathPointsCatmull = new List<Vector3>();

    Vector3 previousPoint = Interp(vector3s, 0);
    int SmoothAmount = path.Length * 15;
    for (int i = 1; i <= SmoothAmount; i++)
    {
      float pm = (float)i / SmoothAmount;
      Vector3 currPt = Interp(vector3s, pm);

      pathPointsCatmull.Add(previousPoint);
      previousPoint = currPt;
    }

    return pathPointsCatmull;
  }

  private static Vector3[] PathSmoothingAtCurvePoint(Vector3[] path)
  {
    Vector3 first;
    Vector3 middle;
    Vector3 last;

    for (int i = 0; i < path.Length-2; i++)
    {
      first = path[i];
      middle = path[i + 1];
      last = path[i + 2];

      Vector3 targetFirst = (first - middle).normalized;
      Vector3 targetSecond = (middle - last).normalized;

      float angle = Vector3.Angle(targetFirst, targetSecond);
      //Debug.Log(i + "\t " + angle);

      if(angle>80f)
      {
        //Ide jön a first és a last közötti optimális kanyar
        Vector3 midpoint = (first + last) / 2;
        //Debug.Log("midpoint :" + midpoint);

        path[i + 1] =(middle + midpoint) / 2;
        //Debug.Log("middle modified: " + path[i + 1]);
      }


    }
    return path;
  }
  private static Vector3[] PathControlPointGenerator(Vector3[] path)
  {
    Vector3[] suppliedPath;
    Vector3[] vector3s;

    //create and store path points:
    suppliedPath = path;

    int offset = 2;
    vector3s = new Vector3[suppliedPath.Length + offset];
    Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

    vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
    vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

    //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
    if (vector3s[1] == vector3s[vector3s.Length - 2])
    {
      Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
      Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
      tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
      tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
      vector3s = new Vector3[tmpLoopSpline.Length];
      Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
    }

    return (vector3s);
  }

  private static Vector3 Interp(Vector3[] pts, float t)
  {
    int numSections = pts.Length - 3;
    int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
    float u = t * (float)numSections - (float)currPt;

    Vector3 a = pts[currPt];
    Vector3 b = pts[currPt + 1];
    Vector3 c = pts[currPt + 2];
    Vector3 d = pts[currPt + 3];

    return .5f * (
      (-a + 3f * b - 3f * c + d) * (u * u * u)
      + (2f * a - 5f * b + 4f * c - d) * (u * u)
      + (-a + c) * u
      + 2f * b
    );
  }
}
