using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RelationshipSquare
{
    public Vector2Int _index;
    public LinearPiecewiseFunction _relationship;
    public float _weight;

    public AnimationCurve curve = new AnimationCurve();

    public RelationshipSquare(GameManager gm, Vector2Int index, float maxInfluenceWeight)
    {
        _index = index;
        _weight = maxInfluenceWeight;

        _relationship = new LinearPiecewiseFunction(gm, maxInfluenceWeight);

        Test();
    }

    public void ChangeWeight(float weight)
    {
        _weight = _relationship.EditMaxInfluenceWeight(weight);
    }

    public void Test()
    {
        // Clear curve
        curve = new AnimationCurve();

        for (int i = 0; i < 100; i++)
        {
            float t = i / 100f;
            curve.AddKey(t, _relationship.Evaluate(t));
        }
    }
}
