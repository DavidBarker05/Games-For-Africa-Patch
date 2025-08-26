using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }

    [SerializeField, Min(0)]
    int numStartingAnimals;
    [SerializeField]
    LayerMask animalLayer;
    [SerializeField]
    List<AnimalAgent> spawnableAnimals = new List<AnimalAgent>();
    [SerializeField]
    List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]
    List<Transform> targets = new List<Transform>();

    List<Transform> availableStartingSpawns;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        if (spawnPoints.Count == 0) return;
        availableStartingSpawns = new List<Transform>(spawnPoints);
        if (numStartingAnimals != spawnPoints.Count) numStartingAnimals = spawnPoints.Count;
        for (int i = 0; i < numStartingAnimals; ++i) SpawnAnimal();
    }

    void SpawnAnimal()
    {
        if (spawnableAnimals.Count == 0 || spawnPoints.Count == 0) return;
        AnimalAgent agent = Instantiate(spawnableAnimals[Random.Range(0, spawnableAnimals.Count)]);
        int index = Random.Range(0, availableStartingSpawns.Count);
        Vector3 spawnPoint = availableStartingSpawns[index].position;
        availableStartingSpawns.RemoveAt(index);
        agent.transform.position = spawnPoint;
    }

    public Vector3 GetNewTarget() => targets.Count > 0 ? targets[Random.Range(0, targets.Count)].position : Vector3.positiveInfinity;
}
