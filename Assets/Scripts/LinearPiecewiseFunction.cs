using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearPiecewiseFunction
{
    private Vector2[] _points;

    public LinearPiecewiseFunction(Vector2[] points)
    {
        // Check for valid points
        if (!IsValidFunction(points))
        {
            return;
        }

        // Normalize x values
        float minX = points[0].x;
        float maxX = points[3].x;

        for (int i = 0; i < points.Length; i++)
        {
            points[i].x = (points[i].x - minX) / (maxX - minX);
        }

        // Set points
        _points = points;
    }

    public float Evaluate(float t)
    {
        // Check for valid function
        if (!IsValidFunction(_points))
        {
            return 0;
        }

        // Interpolate y values
        float a = Mathf.Lerp(_points[0].y, _points[1].y, t);
        float b = Mathf.Lerp(_points[1].y, _points[2].y, t);
        float c = Mathf.Lerp(_points[2].y, _points[3].y, t);

        float d = Mathf.Lerp(a, b, t);
        float e = Mathf.Lerp(b, c, t);

        // Final interpolation to get the result
        return Mathf.Lerp(d, e, t);
    }

    private bool IsValidFunction(Vector2[] points)
    {
        if (points == null || points.Length != 4)
        {
            Debug.LogError("Invalid or uninitialized linear function.");
            return false;
        }
        else
        {
            return true;
        }
    }
}
