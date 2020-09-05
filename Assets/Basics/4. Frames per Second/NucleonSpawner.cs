using UnityEngine;

public class NucleonSpawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float spawnDistance;
    [SerializeField] private Nucleon[] nucleonPrefabs;

    private float _timeSinceLastSpawn;

    private void FixedUpdate()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn >= timeBetweenSpawns)
        {
            _timeSinceLastSpawn -= timeBetweenSpawns;
            SpawnNucleon();
        }
    }

    private void SpawnNucleon()
    {
        var nucleonPrefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        var spawnedNucleon = Instantiate(nucleonPrefab);
        spawnedNucleon.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
