using UnityEngine;
using UnityEngine.Tilemaps;

public class FoodManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public GameObject foodPrefab;

    [Header("Spawn Settings")]
    public bool spawnOnEveryTile = true;
    public TileBase foodGroundTile;

    void Start()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(cell);

            if (tile == null)
                continue;

            // ❗️СПАВНИМ ТОЛЬКО НА НУЖНОМ ТАЙЛЕ
            if (tile != foodGroundTile)
                continue;

            Vector3 worldPos = groundTilemap.GetCellCenterWorld(cell);

            // Берём Z из префаба
            Vector3 spawnPos = new Vector3(
                worldPos.x,
                worldPos.y,
                foodPrefab.transform.position.z
            );

            Instantiate(foodPrefab, spawnPos, Quaternion.identity, transform);
        }
    }



}
