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

    void Awake()
    {
        aiController = GetComponent<AIController>();
    }

    void Update()
    {
        LookForPlayer();
    }

    void LookForPlayer()
    {
        AIState currentState = aiController.GetAIState();

        // ---------------- SEARCH ----------------
        if (currentState == AIState.Search)
        {
            if (CanSeePlayer(out _))
            {
                loseTimer = 0f;
                aiController.SetAIState(AIState.Chase);
            }
        }

        // ---------------- CHASE ----------------
        else if (currentState == AIState.Chase)
        {
            if (CanSeePlayer(out _))
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

        Vector2 center = transform.position;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            Vector2 playerPos = player.transform.position;

            // Проверка зоны обзора
            if (Mathf.Abs(playerPos.x - center.x) > viewWidth / 2) continue;
            if (Mathf.Abs(playerPos.y - center.y) > viewHeight / 2) continue;

            Vector2 dir = (playerPos - center).normalized;
            float dist = Vector2.Distance(center, playerPos);

            // Проверка стен
            if (!Physics2D.Raycast(center, dir, dist, obstacleLayer))
            {
                seenPlayer = player.transform;
                return true;
            }
        }

        return false;
    }


    // Для удобства: показываем область обзора в Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireCube(transform.position, new Vector3(viewWidth, viewHeight, 0f));
    }
}
