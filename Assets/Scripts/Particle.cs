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

        // Check bounds and adjust position if outside
        Vector3 pos = transform.position;
        float radius = _gm._radius;
        Vector4 bounds = _gm._bounds;

        if (pos.x - radius < bounds.x) { pos.x = bounds.x + radius; _velocity.x = -_velocity.x; }
        if (pos.y - radius < bounds.z) { pos.y = bounds.z + radius; _velocity.y = -_velocity.y; }
        if (pos.x + radius > bounds.y) { pos.x = bounds.y - radius; _velocity.x = -_velocity.x; }
        if (pos.y + radius > bounds.w) { pos.y = bounds.w - radius; _velocity.y = -_velocity.y; }

        transform.position = pos;
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
