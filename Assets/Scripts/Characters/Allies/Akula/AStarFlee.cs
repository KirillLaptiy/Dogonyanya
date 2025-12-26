using System.Collections.Generic;
using UnityEngine;

public class AStarFlee : AIStateBase
{
    [Header("Threat")]
    public Transform threat;
    public string enemyTag = "Enemy";

    [Header("Flee Settings")]
    public float fleeDistance = 8f;
    public float recalcInterval = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        if (threat == null)
            threat = GameObject.FindGameObjectWithTag(enemyTag)?.transform;
    }

    public override void OnStateEnter()
    {
    }

    public override void Tick()
    {
        if (threat == null || groundTilemap == null || obstacleTilemap == null)
            return;

        Vector3Int startCell = WorldToCell(transform.position);
        Vector3Int fleeTarget;

        bool seeDirectly = CanSeeThreatDirectly();
        if (seeDirectly)
        {
            // Постоянно пересчитываем цель, чтобы держать дистанцию
            fleeTarget = FindFleeTargetMaxDistance(startCell);
        }
        else
        {
            fleeTarget = FindFleeTargetStep(startCell);
        }

        path = FindPath(startCell, fleeTarget);
        currentNode = 0;

        MoveAlongPath();
    }


    // 🔹 Постоянно ориентируемся от текущей позиции врага
    Vector3Int FindFleeTargetMaxDistance(Vector3Int startCell)
    {
        Vector3 startWorld = CellToWorld(startCell);
        Vector3 threatPos = threat.position;

        // Вектор от врага к АИ
        Vector3 dir = (startWorld - threatPos).normalized;

        // Добавим рандомный угол ±15° для естественности
        float angleOffset = Random.Range(-15f, 15f);
        dir = Quaternion.Euler(0, 0, angleOffset) * dir;

        // Цель на fleeDistance от АИ в этом направлении
        Vector3 worldTarget = startWorld + dir * fleeDistance;
        Vector3Int targetCell = WorldToCell(worldTarget);

        // Если недоступно, ищем ближайшую доступную клетку в радиусе fleeDistance
        if (!IsWalkable(targetCell))
            targetCell = FindNearestWalkable(startCell, targetCell, Mathf.CeilToInt(fleeDistance));

        return targetCell;
    }



    // ✅ Умеренный шаг, имитация «неидеального» Flee
    Vector3Int FindFleeTargetStep(Vector3Int startCell)
    {
        Vector3 dir = (CellToWorld(startCell) - threat.position).normalized;

        // случайная дистанция между половиной и полной fleeDistance
        float stepDistance = Random.Range(fleeDistance * 0.5f, fleeDistance);

        // случайный угол ±30°
        float angleOffset = Random.Range(-30f, 30f);
        dir = Quaternion.Euler(0, 0, angleOffset) * dir;

        Vector3 worldTarget = CellToWorld(startCell) + dir * stepDistance;
        Vector3Int targetCell = WorldToCell(worldTarget);

        if (!IsWalkable(targetCell))
            targetCell = FindNearestWalkable(startCell, targetCell, Mathf.CeilToInt(stepDistance));

        return targetCell;
    }

    // 🔹 Проверка доступных клеток
    Vector3Int FindNearestWalkable(Vector3Int from, Vector3Int desired, int radius)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();
        for (int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int c = new Vector3Int(desired.x + x, desired.y + y, 0);
                if (IsWalkable(c)) candidates.Add(c);
            }

        if (candidates.Count == 0) return from;
        return candidates[Random.Range(0, candidates.Count)];
    }

    bool IsWalkable(Vector3Int cell)
    {
        return groundTilemap.HasTile(cell) && !obstacleTilemap.HasTile(cell);
    }

    Vector3 CellToWorld(Vector3Int cell)
    {
        return groundTilemap.GetCellCenterWorld(cell);
    }

    // 🔹 Враг виден напрямую через свободное пространство
    bool CanSeeThreatDirectly()
    {
        Vector2 start = transform.position;
        Vector2 targetPos = threat.position;
        Vector2 dir = (targetPos - start).normalized;
        float dist = Vector2.Distance(start, targetPos);

        // проверка препятствий
        return !Physics2D.Raycast(start, dir, dist, LayerMask.GetMask("Obstacle"));
    }
}
