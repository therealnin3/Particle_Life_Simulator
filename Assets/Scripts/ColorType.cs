using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorType
{
    public GameManager _gm;
    public Color _color;
    public LinearPiecewiseFunction[] _relationships;

    public ColorType(GameManager gm, Color color, float[] weights)
    {
        _gm = gm;
        _color = color;
        _relationships = new LinearPiecewiseFunction[_gm._colorPalette.Length];

        //  1       .
        //         / \
        //  0  ___/___\_ _
        //       /
        // -1   /

        for (int i = 0; i < _relationships.Length; i++)
        {
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0, -1f);
            points[1] = new Vector2(_gm._radius, 0);
            points[2] = new Vector2(_gm._maxInfluenceRadius, weights[i]);
            points[3] = new Vector2(_gm._maxDetectionRadius, 0);

            _relationships[i] = new LinearPiecewiseFunction(points);
        }
    }
}


