using System.Collections.Generic;
using UnityEngine;

public class InterestAreaSpawner : MonoBehaviour
{
    public GameObject hideSpotPrefab;
    public int maxSpawnPerArea = 3;
    public float minDistanceBetweenObjects = 2f;
    public int maxAttemptsPerObject = 10;
    public float heightOffset = -0.1f; 

    private List<Collider> interestAreas = new List<Collider>();

    void Start()
    {
        GameObject[] areas = GameObject.FindGameObjectsWithTag("InterestArea");
        foreach (var area in areas)
        {
            Collider col = area.GetComponent<Collider>();
            if (col != null)
                interestAreas.Add(col);
        }

        foreach (var area in interestAreas)
        {
            SpawnInArea(area, maxSpawnPerArea);
        }
    }

    void SpawnInArea(Collider areaCollider, int maxSpawn)
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < maxSpawn && attempts < maxSpawn * maxAttemptsPerObject)
        {
            if (!TryGetValidSpawnPoint(areaCollider, out Vector3 randomPos))
            {
                attempts++;
                continue;
            }

            bool canSpawn = !Physics.CheckSphere(randomPos, minDistanceBetweenObjects);

            if (canSpawn)
            {
                Instantiate(hideSpotPrefab, randomPos, Quaternion.identity);
                spawned++;
            }

            attempts++;
        }
    }

    bool TryGetValidSpawnPoint(Collider col, out Vector3 validPoint)
    {
        Vector3 point = new Vector3(
            Random.Range(col.bounds.min.x, col.bounds.max.x),
            col.bounds.max.y + 5f,
            Random.Range(col.bounds.min.z, col.bounds.max.z)
        );

        Ray ray = new Ray(point, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            validPoint = hit.point + Vector3.up * heightOffset;
            return true;
        }

        validPoint = Vector3.zero;
        return false;
    }
}
