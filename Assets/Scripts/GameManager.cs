using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("---Prefabs---")]
    [SerializeField] private GameObject _particlePrefab;

    [Header("---Color Palette---")]
    [SerializeField] private Color[] _colorPalette = new Color[] { Color.red };

    [Header("---Particle Settings---")]
    [SerializeField] private float _radius; // static
    [SerializeField] private float _maxInfluenceRadius; // static
    [SerializeField] private float _maxDetectionRadius; // static 
    [SerializeField] private int _particleCount = 20;

    private List<GameObject> _particles = new List<GameObject>();

    private void Start()
    {
        SetStaticVariables();
        SpawnParticles(_particleCount);
    }

    private void SetStaticVariables()
    {
        Particle._radius = _radius;
        Particle._maxInfluenceRadius = _maxInfluenceRadius;
        Particle._maxDetectionRadius = _maxDetectionRadius;
    }

    private void SpawnParticles(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnParticle(_colorPalette[Random.Range(0, _colorPalette.Length)]);
        }
    }

    private void SpawnParticle(Color color)
    {
        Vector3 randomPos = new Vector3(Random.Range(SpawnArea._bounds.x, SpawnArea._bounds.y), Random.Range(SpawnArea._bounds.z, SpawnArea._bounds.w), 0f);
        GameObject particle = Instantiate(_particlePrefab, randomPos, Quaternion.identity);
        particle.GetComponent<Particle>().Init(color);

        // Add particle to list
        _particles.Add(particle);
    }
}
