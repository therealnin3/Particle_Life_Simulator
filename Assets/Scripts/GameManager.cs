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
    [SerializeField] public int _colorCount = 1;

    [Header("---Particle Settings---")]
    [SerializeField] public float _radius = 0.3f;
    [SerializeField] public float _maxInfluenceRadius = 1f; // NOTE: MUST BE > _radius + _radius
    [SerializeField] public float _maxDetectionRadius = 2f; // NOTE: MUST BE > _maxInfluenceRadius
    [SerializeField] public float _friction = 0.1f; // the higher the value, the more friction

    [Header("---Particles Settings---")]
    [SerializeField] private int _particleCount = 20;

    private List<Particle> _particles = new List<Particle>();
    public RelationshipSquare[][] _weights;

    // Optimazation
    private GridCell[,] _grid;
    private float _cellSize;
    private int _gridXCount;
    private int _gridYCount;

    private void OnValidate()
    {
        // Validate radius' of particle
        float _radiusOffset = _radius * 0.2f; // force 20% offset
        _maxInfluenceRadius = Mathf.Max(_maxInfluenceRadius, _radius + _radius + _radiusOffset);
        _maxDetectionRadius = Mathf.Max(_maxDetectionRadius, _maxInfluenceRadius + _radiusOffset);
    }

    // Gizmos
    private void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_spawnAreaCenter, _spawnAreaRatio);

        // Draw _grid
        if (_grid != null)
        {
            foreach (GridCell c in _grid)
            {
                if (c.particles.Count > 0)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(c.position, new Vector3(_cellSize, _cellSize, 0f));
                }
            }
        }
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

        // Create optimization _grid
        CreateOptimizationGrid();
    }

    private void Update()
    {
        // Update _grid
        UpdateGrid();

        // Apply influence (Optimized)
        foreach (Particle p in _particles)
        {
            // Get _grid position
            int x = Mathf.FloorToInt((p.transform.position.x - _bounds.x) / _cellSize);
            int y = Mathf.FloorToInt((p.transform.position.y - _bounds.z) / _cellSize);

            // Get particles in _grid and surrounding _grid
            List<Particle> neighbours = new List<Particle>();
            for (int i = -1; i <= 1; i++) // x
            {
                for (int j = -1; j <= 1; j++) // y
                {
                    int checkX = x + i;
                    int checkY = y + j;

                    if (checkX >= 0 && checkX < _gridXCount && checkY >= 0 && checkY < _gridYCount)
                    {
                        neighbours.AddRange(_grid[checkX, checkY].particles);
                    }
                }
            }

            // Apply influence
            foreach (Particle n in neighbours)
            {
                if (n != p)
                {
                    ApplyInfluence(p, n);
                }
            }
        }
    }

    private void CreateOptimizationGrid()
    {
        _cellSize = _maxDetectionRadius;
        _gridXCount = Mathf.CeilToInt(_spawnAreaRatio.x / _cellSize);
        _gridYCount = Mathf.CeilToInt(_spawnAreaRatio.y / _cellSize);
        _grid = new GridCell[_gridXCount, _gridYCount];

        Vector2 topLeftCorner = new Vector2(_bounds.x, _bounds.z);

        // Create _grid
        for (int x = 0; x < _gridXCount; x++)
        {
            for (int y = 0; y < _gridYCount; y++)
            {
                Vector2 cellCenter = topLeftCorner + new Vector2(x * _cellSize + (_cellSize / 2), y * _cellSize + (_cellSize / 2));
                _grid[x, y] = new GridCell(cellCenter);
            }
        }
    }

    private void UpdateGrid()
    {
        // Clear _grid
        foreach (GridCell c in _grid)
        {
            c.particles.Clear();
        }

        // Add particles to _grid
        foreach (Particle p in _particles)
        {
            // Get _grid position
            int x = Mathf.FloorToInt((p.transform.position.x - _bounds.x) / _cellSize);
            int y = Mathf.FloorToInt((p.transform.position.y - _bounds.z) / _cellSize);

            // Add particle to _grid
            if (x >= 0 && x < _gridXCount && y >= 0 && y < _gridYCount)
            {
                _grid[x, y].particles.Add(p);
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

        int colorIndex = Random.Range(0, _colorCount);
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
            receiverParticle._velocity += direction * weight * Time.deltaTime;

            // Cap the velocity
            float maxVelocity = 0.7f; // Set your desired maximum velocity here
            receiverParticle._velocity = Vector2.ClampMagnitude(receiverParticle._velocity, maxVelocity);
        }
    }

    private void SetWeights()
    {
        _weights = new RelationshipSquare[_colorCount][];
        for (int i = 0; i < _colorCount; i++)
        {
            _weights[i] = new RelationshipSquare[_colorCount];
            for (int j = 0; j < _colorCount; j++)
            {
                // _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), 0f); // no influence by default
                // _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), 1f); // attraction by default
                // _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), 0f); // repulsion by default
                _weights[i][j] = new RelationshipSquare(this, new Vector2Int(i, j), Random.Range(-1f, 1f));
            }
        }
    }
}
