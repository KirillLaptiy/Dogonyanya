using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class AStarChase : AIStateBase
{
    [Header("Target")]
    public Transform target; // цель для погони (можно задать в инспекторе)

    [Header("Pathfinding")]
    public float recalcInterval = 0.1f;

    private float recalcTimer;

    protected override void Awake()
    {
        base.Awake();

        // Если цель не задана в инспекторе, ищем игрока
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void Tick()
    {
        if (target == null || groundTilemap == null || obstacleTilemap == null)
            return;

        // Пересчёт пути с интервалом
        recalcTimer -= Time.deltaTime;
        if (recalcTimer <= 0f)
        {
            Vector3Int startCell = WorldToCell(transform.position);
            Vector3Int targetCell = WorldToCell(target.position);
            path = FindPath(startCell, targetCell);
            currentNode = 0;

            recalcTimer = recalcInterval;
        }

        // Двигаемся по пути
        MoveAlongPath();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < path.Count - 1; i++)
                Gizmos.DrawLine(path[i], path[i + 1]);

            Gizmos.color = Color.yellow;
            foreach (var node in path)
                Gizmos.DrawSphere(node, 0.08f);

            if (currentNode < path.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(path[currentNode], 0.12f);
            }
        }

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
#endif
}
