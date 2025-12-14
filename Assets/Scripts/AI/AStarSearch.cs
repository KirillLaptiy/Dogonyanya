using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class AStarSearch : AIStateBase
{
    [Header("Search Settings")]
    public float searchInterval = 5f; // пересчёт пути каждые 5 секунд
    public float speedIncrement = 0.1f; // ускорение за каждую смену пути
    public float maxSpeed = 7f; // ускорение за каждую смену пути
    public int targetPlayerEvery = 3; // каждая 3-я итерация на игрока

    private float searchTimer = 0f;
    private Vector3 targetPosition;
    private int iterationCount = 0;
    private Transform player;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        PickRandomTarget();
    }

    public override void Tick()
    {
        if (groundTilemap == null || obstacleTilemap == null) return;

        searchTimer -= Time.deltaTime;

        // Если дошли до конца пути или прошло время
        bool needNewPath = path.Count == 0 || currentNode >= path.Count || searchTimer <= 0f;
        if (needNewPath)
        {
            iterationCount++;

            // Каждая N-я итерация выбираем игрока как цель
            if (player != null && iterationCount % targetPlayerEvery == 0)
            {
                targetPosition = player.position;
            }
            else
            {
                PickRandomTarget();
            }

            RecalculatePath();
            searchTimer = searchInterval;

            // Ускоряем врага
            if (speed < maxSpeed) { speed += speedIncrement; }
        }

        MoveAlongPath();
    }

    void PickRandomTarget()
    {
        if (groundTilemap == null) return;

        Vector3 randomPos;
        int attempts = 0;

        do
        {
            float randX = Random.Range(groundTilemap.cellBounds.xMin, groundTilemap.cellBounds.xMax);
            float randY = Random.Range(groundTilemap.cellBounds.yMin, groundTilemap.cellBounds.yMax);
            randomPos = groundTilemap.CellToWorld(new Vector3Int((int)randX, (int)randY, 0));

            attempts++;
            if (attempts > 50) break; // чтобы не зациклиться

        } while (!groundTilemap.HasTile(groundTilemap.WorldToCell(randomPos)) ||
                 obstacleTilemap.HasTile(groundTilemap.WorldToCell(randomPos)));

        targetPosition = randomPos;
    }

    void RecalculatePath()
    {
        Vector3Int startCell = WorldToCell(transform.position);
        Vector3Int targetCell = WorldToCell(targetPosition);

        var newPath = FindPath(startCell, targetCell);
        if (newPath.Count == 0) return;

        // Убираем первый узел, если слишком близко
        if (Vector3.Distance(newPath[0], rb.position) < reachThreshold)
            newPath.RemoveAt(0);

        path = newPath;
        currentNode = 0;
    }
}
