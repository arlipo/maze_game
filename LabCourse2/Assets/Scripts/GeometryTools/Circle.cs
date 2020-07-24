using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Circle
{
    public static Vector3[] Create(Vector3 center, int radius, int edgeCount) {
        var result = new Vector3[edgeCount];
        var oneAngle = 2 * Mathf.PI / edgeCount;
        for (int i = 0; i < edgeCount; i++)
        {
            var angle = oneAngle * i;
            var x = center.x + radius * Mathf.Sin(angle);
            var z = center.z + radius * Mathf.Cos(angle);
            result[i] = new Vector3(x, z, 0);   
        }
        return result;
    }
}
