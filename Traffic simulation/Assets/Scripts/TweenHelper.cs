using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TweenHelper : MonoBehaviour
{
  public Transform[] test;
  public Vector3[] testV3;
  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  private void OnDrawGizmos()
  {
    Vector3[] suppliedLine = new Vector3[test.Length];
    for (int i = 0; i < test.Length; i++)
    {
      suppliedLine[i] = test[i].position;
    }
    testV3 = suppliedLine;
    DrawForwardPath(testV3, Color.red);
  }

  private static void DrawForwardPath(Vector3[] path, Color color)
  {
    Vector3[] vector3s = PathSmoothingAtCurvePoint(PathControlPointGenerator(path));

    foreach (var item in vector3s)
    {
      Debug.Log(item);
    }

    //Line Draw:
    Vector3 previousPoint = Interp(vector3s, 0);
    Gizmos.color = color;
    int SmoothAmount = path.Length * 100;
    for (int i = 1; i <= SmoothAmount; i++)
    {
      float pm = (float)i / SmoothAmount;
      Vector3 currPt = Interp(vector3s, pm);
      Gizmos.DrawLine(currPt, previousPoint);

      previousPoint = currPt;
    }
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
      Debug.Log(i + "\t " + angle);

      if(angle==90f)
      {
        //na most kell a Bezier. Ide jön a first és a last közötti optimális kanyar
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
