using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilVector
{
    public static float Distance(Vector2 vector1, Vector2 vector2)
    {
        return Mathf.Abs(Vector2.Distance(vector1, vector2));
    }

    public static float Angle360(Vector3 from, Vector3 to)
    {
        Vector3 v3 = Vector3.Cross(from, to);
        if (v3.y > 0)
            return Vector3.Angle(from, to);
        else
            return 360 - Vector3.Angle(from, to);
    }
}
