using System.Collections.Generic;
using UnityEngine;

public class ParasitFlee : AIStateBase
{
    [Header("Flee Settings")]
    public float fleeDistance = 8f;
    public float recalcInterval = 0.2f;
    public CanvasGroup eatCanvasGroup;
    
    private float recalcTimer;
    private float recalcTimers;
    private Vector3Int currentTargetCell;
    private ParasitVision vision;
    private Transform threat;

    protected override void Awake()
    {
        base.Awake();
        vision = GetComponent<ParasitVision>();
    }

    public override void OnStateEnter()
    {
        eatCanvasGroup.alpha = 0;
        recalcTimer = 0f;
        currentTargetCell = WorldToCell(transform.position);
    }

    public override void Tick()
    {
        threat = vision.GetTarget();
        
        if (threat == null || groundTilemap == null || obstacleTilemap == null)
            return;

        recalcTimer -= Time.deltaTime;

        Vector3Int startCell = WorldToCell(transform.position);
        float distToTarget = Vector3.Distance(CellToWorld(currentTargetCell), transform.position);

        // Пересчитываем цель только если нужно
        if (recalcTimer <= 0f || distToTarget < 0.5f || Vector3.Distance(threat.position, CellToWorld(currentTargetCell)) < fleeDistance)
        {
            currentTargetCell = CalculateSafeFleeCell(startCell);
            path = FindPath(startCell, currentTargetCell);
            currentNode = 0;
            recalcTimer = recalcInterval;
        }

        MoveAlongPath();
    }

    // 🔹 Рассчитываем безопасную клетку для побега
    Vector3Int CalculateSafeFleeCell(Vector3Int startCell)
    {
        Vector3 startWorld = CellToWorld(startCell);
        Vector3 threatPos = threat.position;

        // Направление от угрозы
        Vector3 dir = (startWorld - threatPos).normalized;

        // Добавляем небольшой рандомный угол для естественности
        float angleOffset = Random.Range(-15f, 15f);
        dir = Quaternion.Euler(0, 0, angleOffset) * dir;

        // Цель на fleeDistance от текущей позиции
        Vector3 worldTarget = startWorld + dir * fleeDistance;

        Vector3Int targetCell = WorldToCell(worldTarget);

        // Проверяем, можно ли дойти
        if (!IsWalkable(targetCell))
            targetCell = FindNearestWalkable(startCell, targetCell, Mathf.CeilToInt(fleeDistance));

        return targetCell;
    }

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

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void IncreaseSpeed(float speedToAdd)
    {
        SetSpeed(GetSpeed() + speedToAdd);
    }

    public void DecreaseSpeed(float speedToDecrease)
    {
        SetSpeed(GetSpeed() - speedToDecrease);
    }

    public float GetSpeed()
    {
        return speed;
    }
}
