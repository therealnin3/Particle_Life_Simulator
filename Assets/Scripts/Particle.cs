using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Static variables
    public static float _radius;
    public static float _influenceRadius;
    public static Color[] _colorPallet;
    public static Vector4 _bounds; // {minX, maxX, minY, maxY}
    public static float _friction;
    public static float _forceMultiplier;
    public static float[][] _colorWeights;

    // Instance variables
    public Color _color;
    public Vector3 _velocity;
    public System.Func<float, float>[] _forceFunctions;

    // PRIVATE METHODS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _influenceRadius);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (transform.position.x < _bounds.x || transform.position.x > _bounds.y)
        {
            _velocity.x *= -1;
        }
        if (transform.position.y < _bounds.z || transform.position.y > _bounds.w)
        {
            _velocity.y *= -1;
        }

        _velocity *= 1 - _friction;
        transform.position += _velocity * Time.deltaTime;

    }

    // PUBLIC METHODS
    public void InitParticle(Color color)
    {
        // Set variables
        _color = color;

        // Set sprite color
        GetComponent<SpriteRenderer>().color = color;

        // Set Scale to fit radius
        transform.localScale = Vector3.one * _radius * 2;

        // Set function
        _forceFunctions = new System.Func<float, float>[_colorPallet.Length];
        int colorIndex = GetColorIndex(color);

        // Define piecewise functions for each color
        for (int i = 0; i < _colorPallet.Length; i++)
        {
            float yControlPoint = _colorWeights[colorIndex][i];

            _forceFunctions[i] = t =>
            {
                float y;
                float x_radius = 0.3f;
                float x_influence = 0.65f;

                if (t < x_radius)
                {
                    // Line from (0,-1) to (0.3,0)
                    y = Mathf.Lerp(-1, 0, t / x_radius);
                }
                else if (t < x_influence)
                {
                    // Line from (0.3,0) to (0.65,yControlPoint)
                    y = Mathf.Lerp(0, yControlPoint, (t - x_radius) / (x_influence - x_radius));
                }
                else
                {
                    // Line from (0.65,yControlPoint) to (1,0)
                    y = Mathf.Lerp(yControlPoint, 0, (t - x_influence) / (1 - x_influence));
                }
                return y;
            };
        }
    }

    // PUBLIC STATIC METHODS
    public static void ApplyForces(GameObject receiver, GameObject applier)
    {
        Particle receiverParticle = receiver.GetComponent<Particle>();
        Particle applierParticle = applier.GetComponent<Particle>();

        // Calculate distance
        float distance = Vector3.Distance(receiver.transform.position, applier.transform.position);

        if (distance < _influenceRadius)
        {
            // Calculate direction
            Vector3 direction = (applier.transform.position - receiver.transform.position).normalized;

            // Calculate force
            int receiverColorIndex = GetColorIndex(receiverParticle._color);
            float force = applierParticle._forceFunctions[receiverColorIndex](distance / _influenceRadius) * _forceMultiplier;

            // Apply force
            receiverParticle._velocity += direction * force * Time.deltaTime;
        }
    }

    public static int GetColorIndex(Color color)
    {
        for (int i = 0; i < _colorPallet.Length; i++)
        {
            if (_colorPallet[i] == color)
            {
                return i;
            }
        }
        return -1;
    }
}
