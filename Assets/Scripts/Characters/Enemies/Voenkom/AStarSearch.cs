using System.Collections.Generic;
using System.Linq;
using Characters.Player;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class AStarSearch : AIStateBase
{
    [Header("Search Settings")]
    public float searchInterval = 5f; // �������� ���� ������ 5 ������
    public float speedIncrement = 0.3f; // ��������� �� ������ ����� ����
    public float maxSpeed = 7f; // ��������� �� ������ ����� ����
    public int targetPlayerEvery = 3; // ������ 3-� �������� �� ������

    private float searchTimer = 0f;
    private Vector3 targetPosition;
    private int iterationCount = 0;
    private Transform player;

    protected override void Awake()
    {
        base.Awake();
        player = FindNonBotPlayer().transform;
        PickRandomTarget();
    }

    public override void OnStateEnter()
    {
        iterationCount = 0;
        speed = 2.5f;
        searchTimer = 0f;

        PickRandomTarget();
        RecalculatePath();
    }

    public override void Tick()
    {
        if (groundTilemap == null || obstacleTilemap == null) return;

        searchTimer -= Time.deltaTime;

        // ���� ����� �� ����� ���� ��� ������ �����
        bool needNewPath = path.Count == 0 || currentNode >= path.Count || searchTimer <= 0f;
        if (needNewPath)
        {
            iterationCount++;

            // ������ N-� �������� �������� ������ ��� ����
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

            // �������� �����
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
            if (attempts > 50) break; // ����� �� �����������

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

        // ������� ������ ����, ���� ������� ������
        if (Vector3.Distance(newPath[0], rb.position) < reachThreshold)
            newPath.RemoveAt(0);

        path = newPath;
        currentNode = 0;
    }

    GameObject FindNonBotPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject humanPlayer = players.FirstOrDefault(p =>
            p.TryGetComponent<PlayerStamina>(out var stamina)
        );
        return humanPlayer;
    }
}
