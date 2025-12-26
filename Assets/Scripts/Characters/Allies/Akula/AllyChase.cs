using UnityEngine;

public class AllyChase : AIStateBase
{
    [Header("Pathfinding")]
    public float recalcInterval = 0.1f;

    private Transform target;

    private float recalcTimer;

    private AkulaVision vision;

    protected override void Awake()
    {
        base.Awake();
        vision = GetComponent<AkulaVision>();
    }

    public override void OnStateEnter()
    {
    }

    public override void Tick()
    {
        target = vision.ParasiteTransform;

        if (target == null || groundTilemap == null || obstacleTilemap == null)
            return;

        // �������� ���� � ����������
        recalcTimer -= Time.deltaTime;
        if (recalcTimer <= 0f)
        {
            Vector3Int startCell = WorldToCell(transform.position);
            Vector3Int targetCell = WorldToCell(target.position);
            path = FindPath(startCell, targetCell);
            currentNode = 0;

            recalcTimer = recalcInterval;
        }

        // ��������� �� ����
        MoveAlongPath();
    }
}
