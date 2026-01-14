using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AIStateBase : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2.5f;
    public float reachThreshold = 1f;
    
    [Header("Collision")]
    public LayerMask obstacleMask;

    protected Rigidbody2D rb;
    protected List<Vector3> path = new List<Vector3>();
    protected int currentNode = 0;
    
    protected Tilemap groundTilemap;
    protected Tilemap obstacleTilemap;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    protected virtual void Start()
    {
        groundTilemap = TilemapManager.Instance.groundTilemap;
        obstacleTilemap = TilemapManager.Instance.obstacleTilemap;
    }

    protected void MoveAlongPath()
    {
        if (path.Count == 0 || currentNode >= path.Count) return;

        Vector2 currentPos = rb.position;
        Vector3 targetPos3D = path[currentNode];
        Vector2 targetPos = new Vector2(targetPos3D.x, targetPos3D.y);

        Vector2 moveDir = targetPos - currentPos;
        float distance = moveDir.magnitude;
        if (distance < Mathf.Epsilon) return;

        Vector2 dirNormalized = moveDir / distance;
        Vector2 desiredMove = dirNormalized * speed * Time.fixedDeltaTime;

        Vector2 finalMove = desiredMove;
        RaycastHit2D hitX = Physics2D.Raycast(currentPos, new Vector2(desiredMove.x, 0), Mathf.Abs(desiredMove.x), obstacleMask);
        RaycastHit2D hitY = Physics2D.Raycast(currentPos, new Vector2(0, desiredMove.y), Mathf.Abs(desiredMove.y), obstacleMask);

        if (hitX) finalMove.x = 0;
        if (hitY) finalMove.y = 0;

        rb.MovePosition(currentPos + finalMove);

        if (Vector2.Distance(rb.position, targetPos) < reachThreshold)
            currentNode++;
    }

    protected Vector3Int WorldToCell(Vector3 pos) => groundTilemap.WorldToCell(pos);

    protected List<Vector3> FindPath(Vector3Int startCell, Vector3Int targetCell)
    {
        Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();

        Node startNode = new Node(startCell);
        Node endNode = new Node(targetCell);

        List<Node> openList = new List<Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startCell, targetCell);
        openList.Add(startNode);
        nodes[startCell] = startNode;

        // 8 направлений (включа€ диагонали)
        Vector3Int[] dirs =
        {
            Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right,
            new Vector3Int(1,1,0), new Vector3Int(1,-1,0),
            new Vector3Int(-1,1,0), new Vector3Int(-1,-1,0)
        };

        while (openList.Count > 0)
        {
            Node current = openList[0];
            foreach (Node n in openList)
                if (n.fCost < current.fCost) current = n;

            openList.Remove(current);
            closedSet.Add(current.pos);

            if (current.pos == targetCell)
                return RetracePath(current);

            foreach (var d in dirs)
            {
                Vector3Int neighborPos = current.pos + d;

                // ѕровер€ем, что тайл существует и не €вл€етс€ преп€тствием
                if (!groundTilemap.HasTile(neighborPos) || obstacleTilemap.HasTile(neighborPos))
                    continue;

                // «апрет диагоналей через углы
                if (Mathf.Abs(d.x) + Mathf.Abs(d.y) == 2) // это диагональ
                {
                    Vector3Int checkX = current.pos + new Vector3Int(d.x, 0, 0);
                    Vector3Int checkY = current.pos + new Vector3Int(0, d.y, 0);
                    if (obstacleTilemap.HasTile(checkX) || obstacleTilemap.HasTile(checkY))
                        continue; // не разрешаем диагональ, если хот€ бы одна сторона зан€та
                }

                if (closedSet.Contains(neighborPos))
                    continue;

                int moveCost = (Mathf.Abs(d.x) + Mathf.Abs(d.y)) == 2 ? 14 : 10;
                int tentativeG = current.gCost + moveCost;

                Node neighbor;
                if (!nodes.TryGetValue(neighborPos, out neighbor))
                {
                    neighbor = new Node(neighborPos);
                    nodes[neighborPos] = neighbor;
                }

                if (tentativeG < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighborPos, targetCell);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return new List<Vector3>(); // путь не найден
    }

    protected int Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 10 * (dx + dy) + (14 - 2 * 10) * Mathf.Min(dx, dy);
    }

    protected List<Vector3> RetracePath(Node endNode)
    {
        List<Vector3> result = new List<Vector3>();
        Node current = endNode;
        while (current != null)
        {
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(current.pos);
            worldPos.z = 0f;

            // —двиг ноды от стены (антизалипание)
            Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.15f, obstacleMask);
            if (hit)
            {
                Vector2 dirAway = (worldPos - (Vector3)hit.bounds.center).normalized;
                worldPos += (Vector3)(dirAway);
            }

            result.Add(worldPos);
            current = current.parent;
        }

        result.Reverse();

        // —мещаем все ноды кроме последней на 30-50% к следующей ноде
        for (int i = 0; i < result.Count - 1; i++)
        {
            Vector3 dir = result[i + 1] - result[i];
            result[i] += dir * 1.5f;
        }


        return result;
    }

    public abstract void OnStateEnter();
    public abstract void Tick(); //  аждый стейт реализует свою логику цели
}
