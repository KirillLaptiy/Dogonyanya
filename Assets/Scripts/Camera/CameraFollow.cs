using UnityEngine;

public class CameraFollowDynamic : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Игрок

    [Header("Smooth Follow")]
    public float smoothTime = 0.2f; // скорость плавности

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f; // смещение камеры вперед по движению игрока
    public float lookAheadSpeed = 3f;    // скорость смещения вперед

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentLookAhead = Vector3.zero;
    private Vector3 lastTargetPosition;

    void Start()
    {
        if (target) lastTargetPosition = target.position;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Вычисляем движение игрока
        Vector3 deltaMovement = target.position - lastTargetPosition;

        // Смещение вперед по X и Y (можно только X, если платформер)
        Vector3 targetLookAhead = new Vector3(deltaMovement.x, deltaMovement.y, 0) * lookAheadDistance;

        // Плавное смещение
        currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * lookAheadSpeed);

        // Целевая позиция камеры
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + currentLookAhead;

        // Плавное движение камеры
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Сохраняем предыдущую позицию игрока
        lastTargetPosition = target.position;
    }
}
