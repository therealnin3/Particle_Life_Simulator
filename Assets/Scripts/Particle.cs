using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // GameManager
    GameManager _gm;

    // Local variables
    public ColorType _colorType;
    public Vector2 _velocity;

    // Components
    private SpriteRenderer _sr;

    public void Update()
    {
        // Apply friction
        _velocity *= 1f - _gm._friction;

        // Move
        transform.position += (Vector3)_velocity * Time.deltaTime;
    }

    public void Init(GameManager gm, ColorType colorType)
    {
        _gm = gm;
        _sr = GetComponent<SpriteRenderer>();
        _colorType = colorType;
        _sr.color = colorType._color;
        transform.localScale = Vector3.one * _gm._radius * 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _gm._radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _gm._maxInfluenceRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _gm._maxDetectionRadius);
    }
}
