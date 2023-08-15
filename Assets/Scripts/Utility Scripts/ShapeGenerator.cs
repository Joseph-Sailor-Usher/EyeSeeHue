using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    List<Vector3> newPoints = new List<Vector3>();
    public List<Vector3> GenerateShape(int pointCount, float radius)
    {
        newPoints.Clear();
        //The list we will return after populating
        //The point we will change before pushing to the list
        Vector3 tempPoint;
        //A circle divided by the number of points we have
        float circleSegment = (float)(2.0f * Mathf.PI) / (float)pointCount;
        //Loop once for each point
        for(int i = 0; i < pointCount; i++)
        {
            //Update tempPoint's position
            tempPoint.x = radius * Mathf.Cos((float)i * circleSegment);
            tempPoint.y = 0;
            tempPoint.z = radius * Mathf.Sin((float)i * circleSegment);
            //Add it to the List
            newPoints.Add(tempPoint);
        }
        //Return the populated list of points
        return newPoints;
    }
}
