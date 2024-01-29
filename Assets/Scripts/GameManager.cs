using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("---Prefabs---")]
    [SerializeField] private GameObject _particlePrefab;

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
    private float[][] _weights;

    private void Start()
    {
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
        Vector3 randomPos = new Vector3(Random.Range(SpawnArea._bounds.x, SpawnArea._bounds.y), Random.Range(SpawnArea._bounds.z, SpawnArea._bounds.w), 0f);
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
                _weights[i][j] = Random.Range(-1f, 1f);
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
