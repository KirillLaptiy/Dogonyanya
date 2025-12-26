using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarFoodPlace : AIStateBase
{
    [Header("Values")]
    public int SEARCH_RADIUS = 5;
    public int MIN_LOCAL_CELLS = 3;
    public int MAX_LOCAL_CELLS = 4;

    [Header("Food")]
    public GameObject foodPrefab;
    public TileBase foodGroundTile;

    private Vector3Int targetCell;
    private bool hasTarget;

    public override void OnStateEnter()
    {
        FindNewTarget();
    }

    public override void Tick()
    {
        TryPlaceFood();

        if (!hasTarget)
            FindNewTarget();

        MoveAlongPath();

        if (currentNode >= path.Count)
            hasTarget = false;
    }

    void FindNewTarget()
    {
        Vector3Int startCell = WorldToCell(transform.position);

        List<Vector3Int> foodCells = FindFoodTilesInRadius(startCell);

        // 🔥 Есть food — пытаемся идти к ближайшей
        if (foodCells.Count > 0)
        {
            foodCells.Sort((a, b) =>
            {
                int da = Mathf.Abs(a.x - startCell.x) + Mathf.Abs(a.y - startCell.y);
                int db = Mathf.Abs(b.x - startCell.x) + Mathf.Abs(b.y - startCell.y);
                return da.CompareTo(db);
            });

            foreach (Vector3Int cell in foodCells)
            {
                List<Vector3> testPath = FindPath(startCell, cell);

                if (testPath != null && testPath.Count > 0)
                {
                    targetCell = cell;
                    path = testPath;
                    currentNode = 0;
                    hasTarget = true;
                    return;
                }
            }
        }

        // ❗ FOOD нет или всё недостижимо → просто гуляем
        Vector3Int randomCell = FindRandomReachableCell(startCell);

        if (randomCell != startCell)
        {
            path = FindPath(startCell, randomCell);
            currentNode = 0;
            hasTarget = path != null && path.Count > 0;
        }
        else
        {
            hasTarget = false;
        }
    }




    List<Vector3Int> FindFoodTilesInRadius(Vector3Int center)
    {
        List<Vector3Int> result = new();

        for (int x = -SEARCH_RADIUS; x <= SEARCH_RADIUS; x++)
        {
            for (int y = -SEARCH_RADIUS; y <= SEARCH_RADIUS; y++)
            {
                Vector3Int cell = new(center.x + x, center.y + y, 0);

                if (!groundTilemap.HasTile(cell)) continue;
                if (obstacleTilemap.HasTile(cell)) continue;

                TileBase tile = groundTilemap.GetTile(cell);
                if (tile != foodGroundTile) continue;

                if (HasFood(cell)) continue;

                result.Add(cell);
            }
        }

        return result;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    public void TryPlaceFood()
    {
        Vector3Int currentCell = WorldToCell(transform.position);

        // Проверка тайла
        TileBase tile = groundTilemap.GetTile(currentCell);

        if (tile != foodGroundTile)
            return;

        if (HasFood(currentCell))
            return;

        Vector3 worldPos = groundTilemap.GetCellCenterWorld(currentCell);

        Vector3 spawnPos = new(
            worldPos.x,
            worldPos.y,
            foodPrefab.transform.position.z
        );

        Instantiate(foodPrefab, spawnPos, Quaternion.identity);
    }

    Vector3Int FindRandomReachableCell(Vector3Int startCell, int radius = 10)
    {
        List<Vector3Int> candidates = new();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3Int cell = new(startCell.x + x, startCell.y + y, 0);

                if (!groundTilemap.HasTile(cell)) continue;
                if (obstacleTilemap.HasTile(cell)) continue;

                candidates.Add(cell);
            }
        }

        Shuffle(candidates);

        foreach (var cell in candidates)
        {
            List<Vector3> testPath = FindPath(startCell, cell);

            if (testPath != null && testPath.Count > 0)
                return cell;
        }

        return startCell; // fallback (на месте)
    }


    bool HasFood(Vector3Int cell)
    {
        Vector3 pos = groundTilemap.GetCellCenterWorld(cell);
        return Physics2D.OverlapCircle(pos, 0.2f, LayerMask.GetMask("Food"));
    }


}
