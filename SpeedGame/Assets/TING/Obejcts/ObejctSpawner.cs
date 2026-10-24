using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObejctSpawner : MonoBehaviour
{
    public List<GameObject> objectsToSpawn;
    public List<GameObject> SpawnLocations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject spawnLocation = SpawnLocations[Random.Range(0, SpawnLocations.Count)];

        GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];

        if (objectToSpawn != null && spawnLocation != null)
            Instantiate(objectToSpawn, spawnLocation.transform.position, spawnLocation.transform.rotation);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
