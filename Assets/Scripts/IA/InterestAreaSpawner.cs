using System.Collections.Generic;
using UnityEngine;

public class InterestAreaSpawner : MonoBehaviour
{
    public GameObject hideSpotPrefab;
    public int maxSpawnPerArea = 3;
    public float minDistanceBetweenObjects = 2f;
    public int maxAttemptsPerObject = 10;

    private List<Collider> interestAreas = new List<Collider>();

    void Start()
    {
        GameObject[] areas = GameObject.FindGameObjectsWithTag("InterestArea");
        foreach(var area in areas)
        {
            Collider col = area.GetComponent<Collider>();
            if (col != null && col.isTrigger)
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
            Vector3 randomPos = GetRandomPointInCollider(areaCollider);

            bool canSpawn = !Physics.CheckSphere(randomPos, minDistanceBetweenObjects);

            if (canSpawn)
            {
                Instantiate(hideSpotPrefab, randomPos, Quaternion.identity);
                spawned++;
            }

            attempts++;
        }
    }

    Vector3 GetRandomPointInCollider(Collider col)
    {
        Vector3 point = new Vector3(
            Random.Range(col.bounds.min.x, col.bounds.max.x),
            col.bounds.center.y,
            Random.Range(col.bounds.min.z, col.bounds.max.z)
        );

        Ray ray = new Ray(new Vector3(point.x, 100f, point.z), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            point.y = hit.point.y;
        }

        return point;
    }
}
