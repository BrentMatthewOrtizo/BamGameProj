using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game399.Shared.Diagnostics;
using Game.Runtime;

public class ItemSpawner : MonoBehaviour
{
    
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    [Header("Prefabs")]
    public GameObject foodPrefab;
    public GameObject[] emblemPrefabs; // 3 types

    [Header("Spawn Zones (Assign your polygon colliders)")]
    public PolygonCollider2D[] biomeAreas;

    [Header("Spawn Limits")]
    public int maxFoodPerBiome = 10;
    public int maxEmblemsPerBiome = 15;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float raycastDistance = 15f;

    [Header("Timing")]
    public float spawnInterval = 1f;

    private void Start()
    {
        StartCoroutine(SpawnAllBiomes());
    }

    private IEnumerator SpawnAllBiomes()
    {
        foreach (var biome in biomeAreas)
        {
            if (biome == null) continue;

            // Count items spawned in this biome
            int spawnedFood = 0;
            int spawnedEmblems = 0;

            // keep spawning until biome quotas are filled
            while (spawnedFood < maxFoodPerBiome || spawnedEmblems < maxEmblemsPerBiome)
            {
                // pick random spawn type
                bool spawnFood = Random.value < 0.5f;

                // enforce biome caps
                if (spawnFood && spawnedFood >= maxFoodPerBiome)
                    spawnFood = false;
                if (!spawnFood && spawnedEmblems >= maxEmblemsPerBiome)
                    spawnFood = true;

                // random point inside polygon bounds
                Vector2 randomPoint = GetRandomPointInPolygon(biome);

                // raycast downward to ground
                RaycastHit2D hit = Physics2D.Raycast(randomPoint, Vector2.down, raycastDistance, groundLayer);
                if (hit.collider != null)
                {
                    Vector2 spawnPos = hit.point + Vector2.up * 0.5f;
                    GameObject prefabToSpawn = spawnFood ? foodPrefab : emblemPrefabs[Random.Range(0, emblemPrefabs.Length)];

                    // create item and assign unique ID
                    GameObject newItem = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    string id = $"{prefabToSpawn.name}_{biome.name}_{spawnPos.x:F1}_{spawnPos.y:F1}";
                    AssignUniqueID(newItem, id);

                    // log
                    Log.Info($"Spawned {prefabToSpawn.name} in {biome.name} at {spawnPos}");

                    if (spawnFood) spawnedFood++;
                    else spawnedEmblems++;

                    yield return new WaitForSeconds(spawnInterval);
                }
                else
                {
                    // couldn’t find ground, try again next frame
                    yield return null;
                }
            }
        }

        Log.Info("Finished spawning all biomes.");
    }

    private Vector2 GetRandomPointInPolygon(PolygonCollider2D poly)
    {
        Bounds bounds = poly.bounds;
        Vector2 point;
        int safety = 0;

        do
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            point = new Vector2(randomX, randomY);
            safety++;
        } while (!poly.OverlapPoint(point) && safety < 100);

        return point;
    }

    private void AssignUniqueID(GameObject item, string id)
    {
        // Prevent duplicates if already collected
        if (PickupTracker.Instance != null && PickupTracker.Instance.HasCollected(id))
        {
            Destroy(item);
            return;
        }

        // attach to proper script
        if (item.TryGetComponent(out PickUpItem pickUp))
            pickUp.uniqueID = id;
        else if (item.TryGetComponent(out KeyPickup key))
            key.uniqueID = id;
        else if (item.TryGetComponent(out PortalPickup portal))
            portal.uniqueID = id;
    }
}
