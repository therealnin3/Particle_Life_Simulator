using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("---Prefabs---")]
    [SerializeField] private GameObject _particlePrefab;

    [Header("---Spawn Area---")]
    [SerializeField] private Vector2 _spawnAreaRatio = new Vector2(10, 10); // {Height, Width}
    [SerializeField] private Vector2 _spawnAreaCenter = new Vector2(0, 0); // {x, y}
    [SerializeField] public bool _spawnAreaWrap = false;
    [HideInInspector] public Vector4 _bounds; // {minX, maxX, minY, maxY}

    [Header("---Color Palette---")]
    [SerializeField] public Color[] _colorPalette = new Color[] { Color.red };

    [Header("---Particle Settings---")]
    [SerializeField] public float _radius = 0.3f;
    [SerializeField] public float _maxInfluenceRadius = 1f; // NOTE: MUST BE > _radius
    [SerializeField] public float _maxDetectionRadius = 2f; // NOTE: MUST BE > _maxInfluenceRadius
    [SerializeField] public float _friction = 0.1f; // the higher the value, the more friction

    [Header("---Particles Settings---")]
    [SerializeField] private int _particleCount = 20;

    private List<Particle> _particles = new List<Particle>();
    public RelationshipSquare[][] _weights;


    // Gizmos
    private void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_spawnAreaCenter, _spawnAreaRatio);
    }

    private void Awake()
    {
        // Set spawnArea
        float _minX = _spawnAreaCenter.x - (_spawnAreaRatio.x / 2);
        float _maxX = _spawnAreaCenter.x + (_spawnAreaRatio.x / 2);
        float _minY = _spawnAreaCenter.y - (_spawnAreaRatio.y / 2);
        float _maxY = _spawnAreaCenter.y + (_spawnAreaRatio.y / 2);
        _bounds = new Vector4(_minX, _maxX, _minY, _maxY);

        // Set Weights
        SetWeights();

        // Spawn Particles
        SpawnParticles(_particleCount);
    }

    private void Update()
    {
        foreach (Particle i in _particles)
        {
            foreach (Particle j in _particles)
            {
                if (i != j)
                {
                    ApplyInfluence(i, j);
                }
            }
        }
    }

    private void SpawnParticles(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnParticle();
        }
    }

    private void SpawnParticle()
    {
        Vector3 randomPos = new Vector3(Random.Range(_bounds.x, _bounds.y), Random.Range(_bounds.z, _bounds.w), 0f);
        GameObject particle = Instantiate(_particlePrefab, randomPos, Quaternion.identity);

        int colorIndex = Random.Range(0, _colorPalette.Length);
        Particle p = particle.GetComponent<Particle>();
        p.Init(this, _colorPalette[colorIndex], colorIndex, _weights[colorIndex]);

        // Add particle to list
        _particles.Add(p);
    }

    private void ApplyInfluence(Particle receiverParticle, Particle applierParticle)
    {
        // Get distance between particles
        float distance = Vector3.Distance(receiverParticle.transform.position, applierParticle.transform.position);

        if (distance <= _maxDetectionRadius)
        {
            float t = distance; // Send in UNNORMALIZED distance
            float weight = receiverParticle._relationships[applierParticle._colorIndex]._relationship.Evaluate(t);

            // Apply influence
            Vector2 direction = (applierParticle.transform.position - receiverParticle.transform.position).normalized;
            receiverParticle._velocity += direction * weight * Time.deltaTime * 100f;
        }
    }

    private void SetWeights()
    {
        int colorCount = _colorPalette.Length;
        _weights = new RelationshipSquare[colorCount][];
        for (int i = 0; i < colorCount; i++)
        {
            _weights[i] = new RelationshipSquare[colorCount];
            for (int j = 0; j < colorCount; j++)
            {
                // _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), 0f); // no influence by default
                // _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), 1f); // attraction by default
                // _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), 0f); // repulsion by default
                _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), Random.Range(-1f, 1f));
            }
        }
    }
}
