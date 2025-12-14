using UnityEngine;

public class VoenkomVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewWidth = 20f;      // по X
    public float viewHeight = 20f;     // по Y
    public LayerMask obstacleLayer;   // слой стен/препятствий

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
        // Создаём центр обзора — враг находится в центре
        Vector2 center = transform.position;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            Vector2 playerPos = player.transform.position;

            // Проверяем, находится ли игрок в прямоугольной зоне 3x3
            if (Mathf.Abs(playerPos.x - center.x) > viewWidth / 2) continue;
            if (Mathf.Abs(playerPos.y - center.y) > viewHeight / 2) continue;

            Vector2 direction = (playerPos - center).normalized;
            float distance = Vector2.Distance(center, playerPos);

            // Проверяем наличие препятствий
            if (!Physics2D.Raycast(center, direction, distance, obstacleLayer))
            {
                aiController.currentStateEnum = AIState.Chase;
                return; // игрок найден, переключаемся на Chase
            }
        }

        // Если игрок не найден — можно оставаться в текущем режиме (например, Search)
    }

    // Для удобства: показываем область обзора в Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireCube(transform.position, new Vector3(viewWidth, viewHeight, 0f));
    }
}
