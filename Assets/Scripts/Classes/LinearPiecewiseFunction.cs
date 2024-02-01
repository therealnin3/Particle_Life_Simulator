using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearPiecewiseFunction
{
    private Vector2[] _points;
    private LinearFunction[] _functions;
    private GameManager _gm;

    public LinearPiecewiseFunction(GameManager gm, float maxInfluenceWeight)
    {
        // Reference Gamemanager
        _gm = gm;

        //  1       .
        //         / \
        //  0  ___/___\
        //       /
        // -1   /

        // Generate 4 points and normalize them
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(0, -20f);
        points[1] = new Vector2(gm._radius + gm._radius, 0); // NOTE: gm._radius + gm._radius -> prevents cirlces from overlapping
        points[2] = new Vector2(gm._maxInfluenceRadius, maxInfluenceWeight);
        points[3] = new Vector2(gm._maxDetectionRadius, 0);
        _points = NormalizePoints(points);

        // Generate functions from points
        _functions = GenerateFunctions(_points);
    }

    public float Evaluate(float t)
    {
        // Normalize t
        t /= _gm._maxDetectionRadius;

        // Smaller than radius
        if (t < _points[1].x)
        {
            // return -1;
            return _functions[0].Evaluate(t);
        }
        else if (t < _points[2].x)
        {
            return _functions[1].Evaluate(t);
        }
        else
        {
            return _functions[2].Evaluate(t);
        }
    }

    private LinearFunction[] GenerateFunctions(Vector2[] points)
    {
        LinearFunction[] functions = new LinearFunction[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            functions[i] = new LinearFunction(points[i], points[i + 1]);
        }
        return functions;
    }

    private Vector2[] NormalizePoints(Vector2[] points)
    {
        // Normalize points
        Vector2[] normalizedPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            normalizedPoints[i] = new Vector2(points[i].x / _gm._maxDetectionRadius, points[i].y);
        }
        return normalizedPoints;
    }

    // External use
    public float EditMaxInfluenceWeight(float maxInfluenceWeight)
    {
        // float threshold = 1e-5f;
        // if (Mathf.Abs(maxInfluenceWeight) < threshold)
        // {
        //     maxInfluenceWeight = 0f;
        // }

        _points[2].y = maxInfluenceWeight;
        _functions = GenerateFunctions(_points);

        return maxInfluenceWeight;
    }

    private class LinearFunction
    {
        public float _m;
        private float _b;

        public LinearFunction(Vector2 startPoint, Vector2 endPoint)
        {
            _m = (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
            _b = startPoint.y - _m * startPoint.x;
        }

        public float Evaluate(float x)
        {
            return _m * x + _b;
        }
    }


}