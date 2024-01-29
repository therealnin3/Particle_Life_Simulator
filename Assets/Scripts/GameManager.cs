using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _particlePrefab;

    [Header("Settings")]
    [SerializeField] private float _particleRadius = 0.3f;
    [SerializeField] private float _particleInfluenceRadius = 1f;
    [SerializeField] private float _particleFriction = 0.1f;
    [SerializeField] private int _particleAmount = 100;
    [SerializeField] private float _forceMultiplier = 5f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 _spawnAreaRatio = new Vector2(10, 10); // {Height, Width}
    [SerializeField] private Vector2 _spawnAreaCenter = new Vector2(0, 0); // {x, y}
    private Vector4 _bounds; // {minX, maxX, minY, maxY}

    [Header("Colors")]
    [SerializeField] private Color[] _colorPalette = new Color[] { Color.red };
    private float[][] _colorWeights;

    private List<GameObject> _particles = new List<GameObject>();

    // Gizmos
    private void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_spawnAreaCenter, _spawnAreaRatio);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetupSettings();
        SpawnParticles(_particleAmount);
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (GameObject particle in _particles)
        {
            foreach (GameObject otherParticle in _particles)
            {
                if (particle != otherParticle)
                {
                    Particle.ApplyForces(particle, otherParticle);
                }
            }
        }
    }

    private void PopulateRelationMatrix()
    {
        _colorWeights = new float[_colorPalette.Length][];
        for (int i = 0; i < _colorPalette.Length; i++)
        {
            _colorWeights[i] = new float[_colorPalette.Length];
            for (int j = 0; j < _colorPalette.Length; j++)
            {
                if (i == j)
                {
                    _colorWeights[i][j] = 1;
                }
                else
                {
                    _colorWeights[i][j] = 0;
                }
            }
        }
    }

    private void SetupSettings()
    {
        // Calculate spawn area
        float _minX = _spawnAreaCenter.x - (_spawnAreaRatio.x / 2);
        float _maxX = _spawnAreaCenter.x + (_spawnAreaRatio.x / 2);
        float _minY = _spawnAreaCenter.y - (_spawnAreaRatio.y / 2);
        float _maxY = _spawnAreaCenter.y + (_spawnAreaRatio.y / 2);
        _bounds = new Vector4(_minX, _maxX, _minY, _maxY);

        // Set static variables
        Particle._radius = _particleRadius;
        Particle._influenceRadius = _particleInfluenceRadius;
        Particle._colorPallet = _colorPalette;
        Particle._bounds = _bounds;
        Particle._friction = _particleFriction;
        Particle._forceMultiplier = _forceMultiplier;

        // Set color weights
        PopulateRelationMatrix();
        Particle._colorWeights = _colorWeights;
    }

    private void SpawnParticles(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(_bounds.x, _bounds.y), Random.Range(_bounds.z, _bounds.w), 0);
            Color randomColor = _colorPalette[Random.Range(0, _colorPalette.Length)];
            SpawnParticle(randomColor, randomPos);
        }

    }

    private void SpawnParticle(Color color, Vector3 position)
    {
        GameObject particleObject = Instantiate(_particlePrefab, position, Quaternion.identity);
        particleObject.GetComponent<Particle>().InitParticle(color);

        // Add particle to list
        _particles.Add(particleObject);
    }
}
