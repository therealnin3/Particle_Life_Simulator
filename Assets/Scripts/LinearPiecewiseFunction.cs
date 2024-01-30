using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearPiecewiseFunction
{

    private Vector2[] _points;
    private LinearFunction[] _functions;
    private GameManager _gm;
    private float _diameter;
    private float _diameterCutOff = 0f; // Particles can overlap this %

    public LinearPiecewiseFunction(GameManager gm, float maxInfluenceWeight)
    {
        // Reference Gamemanager
        _gm = gm;
        _diameter = gm._radius * 2f * (1f - _diameterCutOff);

        //  1       .
        //         / \
        //  0  ___/___\
        //       /
        // -1   /

        // Generate 4 points and normalize them
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(0, -1f);
        points[1] = new Vector2(_diameter, 0);
        points[2] = new Vector2(gm._maxInfluenceRadius, maxInfluenceWeight);
        points[3] = new Vector2(gm._maxDetectionRadius, 0);
        _points = NormalizePoints(points);

        // Generate functions from points
        _functions = GenerateFunctions(_points);
    }

    public float Evaluate(float t)
    {
        if (t < _diameter)
        {
            return _functions[0].Evaluate(t);
        }
        else if (t < _gm._maxInfluenceRadius)
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
        float minX = points[0].x;
        float maxX = points[points.Length - 1].x;

        for (int i = 0; i < points.Length; i++)
        {
            points[i].x = (points[i].x - minX) / (maxX - minX);
        }

        return points;
    }

    public void EditMaxInfluenceWeight(float maxInfluenceWeight)
    {
        _points[2].y = maxInfluenceWeight;
        _functions = GenerateFunctions(_points);
    }

    private class LinearFunction
    {
        private float _m;
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