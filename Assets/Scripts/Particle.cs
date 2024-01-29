using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Static variables
    public static float _radius;
    public static float _maxInfluenceRadius;
    public static float _maxDetectionRadius;

    // Local variables
    private Color _color;
    private Vector2 _velocity;

    // Components
    private SpriteRenderer _sr;

    public void Init(Color color)
    {
        _sr = GetComponent<SpriteRenderer>();
        _color = color;
        _sr.color = color;
    }
}
