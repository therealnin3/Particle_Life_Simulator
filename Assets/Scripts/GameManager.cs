using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("---Prefabs---")]
    [SerializeField] private GameObject _particlePrefab;

    [Header("---Spawn Area---")]
    [SerializeField] private Vector2 _spawnAreaRatio = new Vector2(10, 10); // {Height, Width}
    [SerializeField] private Vector2 _spawnAreaCenter = new Vector2(0, 0); // {x, y}
    [HideInInspector] public Vector4 _bounds; // {minX, maxX, minY, maxY}

    [Header("---Color Palette---")]
    [SerializeField] public Color[] _colorPalette = new Color[] { Color.red };

    [Header("---Particle Settings---")]
    [SerializeField] public float _radius = 0.3f;
    [SerializeField] public float _maxInfluenceRadius = 1f;
    [SerializeField] public float _maxDetectionRadius = 2f;
    [SerializeField] public float _friction = 0.1f; // the higher the value, the more friction

    [Header("---Particles Settings---")]
    [SerializeField] private int _particleCount = 20;

    private List<GameObject> _particles = new List<GameObject>();
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
        foreach (GameObject i in _particles)
        {
            foreach (GameObject j in _particles)
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
        particle.GetComponent<Particle>().Init(this, _colorPalette[colorIndex], colorIndex, _weights[colorIndex]);

        // Add particle to list
        _particles.Add(particle);
    }

    private void ApplyInfluence(GameObject receiver, GameObject applier)
    {
        Particle receiverParticle = receiver.GetComponent<Particle>();
        Particle applierParticle = applier.GetComponent<Particle>();

        // Get distance between particles
        float distance = Vector3.Distance(receiver.transform.position, applier.transform.position);

        if (distance <= _maxDetectionRadius)
        {
            float t = distance / _maxDetectionRadius;
            float weight = receiverParticle._relationships[applierParticle._colorIndex]._relationship.Evaluate(t);

            // Apply influence
            Vector2 direction = (applier.transform.position - receiver.transform.position).normalized;
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
