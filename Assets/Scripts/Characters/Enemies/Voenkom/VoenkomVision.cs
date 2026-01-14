using System.Linq;
using Characters.Player;
using UnityEngine;

public class VoenkomVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewWidth = 20f;      // по X
    public float viewHeight = 20f;     // по Y
    public LayerMask obstacleLayer;   // слой стен/препятствий

    [Header("Vision")]
    public float loseTargetTime = 9f;

    private float loseTimer = 0f;

    private AIController aiController;

    private bool isStunned;

    private Transform target;

    void Awake()
    {
        aiController = GetComponent<AIController>();
    }

    void Update()
    {
        if (isStunned) return;
        LookForPlayer();
    }

    void LookForPlayer()
    {
        AIState currentState = aiController.GetAIState();

        // ---------------- SEARCH ----------------
        if (CanSeePlayer(out Transform seenPlayer))
        {
            target = seenPlayer;
        }
        
        if (CanSeeAlly(out Transform seenAlly))
        {
            target = seenAlly;
        }
        
        if (currentState == AIState.Search)
        {
            if (CanSeeAlly(out _) || CanSeePlayer(out _))
            {
                aiController.SetAIState(AIState.Chase);
            }
        }

        // ---------------- CHASE ----------------
        else if (currentState == AIState.Chase)
        {
            if (CanSeePlayer(out _) || CanSeeAlly(out _))
            {
                // Игрок виден → сбрасываем таймер
                loseTimer = 0f;
            }
            else
            {
                // Игрок не виден → накапливаем время
                loseTimer += Time.deltaTime;

                if (loseTimer >= loseTargetTime)
                {
                    loseTimer = 0f;
                    aiController.SetAIState(AIState.Search);
                }
            }
        }
    }

    bool CanSeePlayer(out Transform seenPlayer)
    {
        seenPlayer = null;

        GameObject player = FindNonBotPlayer();
        if (player == null) return false;

        Vector2 center = transform.position;

        Vector2 playerPos = player.transform.position;

        // Проверка зоны обзора
        if (Mathf.Abs(playerPos.x - center.x) > viewWidth / 2) return false;
        if (Mathf.Abs(playerPos.y - center.y) > viewHeight / 2) return false;

        Vector2 dir = (playerPos - center).normalized;
        float dist = Vector2.Distance(center, playerPos);

        // Проверка стен
        if (!Physics2D.Raycast(center, dir, dist, obstacleLayer))
        {
            seenPlayer = player.transform;
            return true;
        }

        return false;
    }

    bool CanSeeAlly(out Transform seenAlly)
    {
        seenAlly = null;

        GameObject player = FindAlly();
        if (player == null) return false;

        Vector2 center = transform.position;

        Vector2 playerPos = player.transform.position;

        // Проверка зоны обзора
        if (Mathf.Abs(playerPos.x - center.x) > viewWidth / 2) return false;
        if (Mathf.Abs(playerPos.y - center.y) > viewHeight / 2) return false;

        Vector2 dir = (playerPos - center).normalized;
        float dist = Vector2.Distance(center, playerPos);

        // Проверка стен
        if (!Physics2D.Raycast(center, dir, dist, obstacleLayer))
        {
            seenAlly = player.transform;
            return true;
        }

        return false;
    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
    }

    public Transform getTarget()
    {
        return target;
    }

    GameObject FindAlly()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject humanPlayer = players.FirstOrDefault(p =>
            p.TryGetComponent<AkulaVision>(out var stamina)
        );
        return humanPlayer;
    }

    GameObject FindNonBotPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject humanPlayer = players.FirstOrDefault(p =>
            p.TryGetComponent<PlayerStamina>(out var stamina)
        );
        return humanPlayer;
    }


    // Для удобства: показываем область обзора в Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireCube(transform.position, new Vector3(viewWidth, viewHeight, 0f));
    }
}
