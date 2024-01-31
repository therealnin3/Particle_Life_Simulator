using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // GameManager
    GameManager _gm;

    // Local variables
    public Vector2 _velocity;
    public int _colorIndex;
    public RelationshipSquare[] _relationships;

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

        // Wrap or bounce
        if (_gm._spawnAreaWrap)
        {
            if (pos.x < bounds.x) { pos.x = bounds.y - (bounds.x - pos.x); }
            if (pos.y < bounds.z) { pos.y = bounds.w - (bounds.z - pos.y); }
            if (pos.x > bounds.y) { pos.x = bounds.x + (pos.x - bounds.y); }
            if (pos.y > bounds.w) { pos.y = bounds.z + (pos.y - bounds.w); }
        }
        else
        {
            if (pos.x - radius < bounds.x) { pos.x = bounds.x + radius; _velocity.x = -_velocity.x; }
            if (pos.y - radius < bounds.z) { pos.y = bounds.z + radius; _velocity.y = -_velocity.y; }
            if (pos.x + radius > bounds.y) { pos.x = bounds.y - radius; _velocity.x = -_velocity.x; }
            if (pos.y + radius > bounds.w) { pos.y = bounds.w - radius; _velocity.y = -_velocity.y; }
        }

        transform.position = pos;
    }

    public void Init(GameManager gm, Color color, int colorIndex, RelationshipSquare[] relationships)
    {
        _gm = gm;
        _relationships = relationships;
        _colorIndex = colorIndex;
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = color;

        // Set radius
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
