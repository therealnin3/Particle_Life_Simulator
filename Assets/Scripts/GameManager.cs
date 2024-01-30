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
    private ColorType[] _colorTypes;

    [Header("---Particle Settings---")]
    [SerializeField] public float _radius = 0.3f;
    [SerializeField] public float _maxInfluenceRadius = 1f;
    [SerializeField] public float _maxDetectionRadius = 2f;
    [SerializeField] public float _friction = 0.1f; // the higher the value, the more friction

    [Header("---Particles Settings---")]
    [SerializeField] private int _particleCount = 20;

    private List<GameObject> _particles = new List<GameObject>();
    public float[][] _weights;


    // Gizmos
    private void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_spawnAreaCenter, _spawnAreaRatio);
    }

    private void Awake()
    {
        float _minX = _spawnAreaCenter.x - (_spawnAreaRatio.x / 2);
        float _maxX = _spawnAreaCenter.x + (_spawnAreaRatio.x / 2);
        float _minY = _spawnAreaCenter.y - (_spawnAreaRatio.y / 2);
        float _maxY = _spawnAreaCenter.y + (_spawnAreaRatio.y / 2);
        _bounds = new Vector4(_minX, _maxX, _minY, _maxY);

        SetWeights();
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
            SpawnParticle(_colorTypes[Random.Range(0, _colorPalette.Length)]);
        }
    }

    private void SpawnParticle(ColorType colorType)
    {
        Vector3 randomPos = new Vector3(Random.Range(_bounds.x, _bounds.y), Random.Range(_bounds.z, _bounds.w), 0f);
        GameObject particle = Instantiate(_particlePrefab, randomPos, Quaternion.identity);
        particle.GetComponent<Particle>().Init(gameObject.GetComponent<GameManager>(), colorType);

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
            int appliedColorIndex = GetColorIndex(applierParticle._colorType._color);
            float weight = receiverParticle._colorType._relationships[appliedColorIndex].Evaluate(t);

            // Apply influence
            Vector2 direction = (applier.transform.position - receiver.transform.position).normalized;
            receiverParticle._velocity += direction * weight * Time.deltaTime * 100f;
        }
    }

    private void SetWeights()
    {
        int colorCount = _colorPalette.Length;
        _weights = new float[colorCount][];
        for (int i = 0; i < colorCount; i++)
        {
            _weights[i] = new float[colorCount];
            for (int j = 0; j < colorCount; j++)
            {

                // _weights[i][j] = Random.Range(-1f, 1f);  // Random weights
                // _weights[i][j] = 1f; // Full attraction
                _weights[i][j] = 0f; // No influence
                // _weights[i][j] = 0.5f; // Full repel 
            }
        }
        CreateColorTypes(colorCount);
    }

    private void CreateColorTypes(int amount)
    {
        _colorTypes = new ColorType[amount];
        for (int i = 0; i < amount; i++)
        {
            _colorTypes[i] = new ColorType(GetComponent<GameManager>(), _colorPalette[i], _weights[i]);
        }
    }

    private int GetColorIndex(Color color)
    {
        for (int i = 0; i < _colorTypes.Length; i++)
        {
            if (_colorPalette[i] == color)
            {
                return i;
            }
        }
        return -1;
    }
}
