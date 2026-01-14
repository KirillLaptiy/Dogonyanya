using System.Linq;
using Characters.Player;
using Unity.Mathematics;
using UnityEngine;

public class ParasitVision : MonoBehaviour
{
    [Header("Vision Settings")] public float viewWidth = 20f;
    public float viewHeight = 20f;
    public LayerMask obstacleLayer;

    private AIController aiController;
    private Transform target;

    void Awake()
    {
        aiController = GetComponent<AIController>();
    }

    void Update()
    {
        Look();
    }

    void Look()
    {
        AIState currentState = aiController.GetAIState();

        // ---------------- Проверяем видимость союзника или игрока ----------------
        if (SeeEnemy(out var enemy))
        {
            target = enemy;

            if (currentState != AIState.ParasitFlee)
            {
                aiController.SetAIState(AIState.ParasitFlee);
            }

            return;
        }

        // ---------------- Если никого не видим → еда ----------------
        if (IsFarEnoughFromTarget() && currentState != AIState.FoodSearch)
        {
            target = null;
            aiController.SetAIState(AIState.FoodSearch);
        }
    }

    private bool SeeEnemy(out Transform closestEnemy)
    {
        closestEnemy = null;

        var canSeePlayer = CanSeeCharacter(false, out var playerTransform);
        var canSeeAlly = CanSeeCharacter(true, out var allyTransform);

        if (!canSeePlayer && !canSeeAlly) return false;

        if (playerTransform != null && allyTransform != null)
        {
            var playerDist = Vector3.Distance(transform.position, playerTransform.position);
            var allyDist = Vector3.Distance(transform.position, allyTransform.position);

            closestEnemy = playerDist <= allyDist ? playerTransform : allyTransform;
        }
        else
        {
            closestEnemy = playerTransform ?? allyTransform;
        }

        return true;
    }

    public bool CanSeeCharacter(bool ally, out Transform seenPlayer)
    {
        seenPlayer = null;

        GameObject player = ally ? FindAlly() : FindNonBotPlayer();
        if (player == null) return false;

        Vector2 center = transform.position;
        Vector2 playerPos = player.transform.position;

        // Проверка зоны обзора
        if (Mathf.Abs(playerPos.x - center.x) > viewWidth / 2) return false;
        if (Mathf.Abs(playerPos.y - center.y) > viewHeight / 2) return false;

        Vector2 dir = (playerPos - center).normalized;
        float dist = Vector2.Distance(center, playerPos);

        // Проверка препятствий
        if (!Physics2D.Raycast(center, dir, dist, obstacleLayer))
        {
            seenPlayer = player.transform;
            return true;
        }

        return false;
    }

    public Transform GetTarget()
    {
        return target;
    }

    GameObject FindAlly()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        return players.FirstOrDefault(p => p != null && p.TryGetComponent<AkulaVision>(out _));
    }

    GameObject FindNonBotPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        return players.FirstOrDefault(p => p != null && p.TryGetComponent<PlayerStamina>(out _));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireCube(transform.position, new Vector3(viewWidth, viewHeight, 0f));
    }

    bool IsFarEnoughFromTarget()
    {
        if (target == null) return true;
        float dist = Vector3.Distance(transform.position, target.position);
        return dist >= (((viewWidth + viewHeight) / 2) + 1f);
    }
}