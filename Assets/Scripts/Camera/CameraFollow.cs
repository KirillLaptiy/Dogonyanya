using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraFollowDynamic : MonoBehaviour
{
    [Header("Target")] public Transform target; // »грок

    [Header("Smooth Follow")] public float smoothTime = 0.2f; // скорость плавности

    [Header("Look Ahead")] public float lookAheadDistance = 2f; // смещение камеры вперед
    public float lookAheadSpeed = 3f; // скорость смещени€ вперед

    [Header("Drunk Effect")] public float wobbleAmount = 0.3f; // амплитуда дрожани€
    public float wobbleSpeed = 2f; // частота дрожани€
    public float rotationAmount = 2f; // максимум поворота камеры в градусах

    private Camera _camera;
    private PixelPerfectCamera ppCamera;
    private Vector3 velocity = Vector3.zero;
    private Vector3 currentLookAhead = Vector3.zero;
    private Vector3 lastTargetPosition;
    private bool _enabled = false;

    private float _wobbleTimer = 0f;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        ppCamera = GetComponent<PixelPerfectCamera>();
    }

    void Start()
    {
        if (target) lastTargetPosition = target.position;
    }

    void LateUpdate()
    {
        if (!_enabled) return; // камера выключена
        if (!target) return; // игрок не задан
        if (Time.timeScale == 0) return; // игра на паузе Ч камера не движетс€

        // ============================
        // ƒвижение камеры
        // ============================
        Vector3 deltaMovement = target.position - lastTargetPosition;
        Vector3 targetLookAhead = new Vector3(deltaMovement.x, deltaMovement.y * 0.5f, 0) * lookAheadDistance;
        currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * lookAheadSpeed);

        Vector3 targetPosition =
            new Vector3(target.position.x, target.position.y, transform.position.z) + currentLookAhead;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // ============================
        // ѕивное дрожание
        // ============================
        _wobbleTimer += Time.deltaTime * wobbleSpeed;
        float wobbleX = Mathf.Sin(_wobbleTimer * 1.5f) * wobbleAmount;
        float wobbleY = Mathf.Cos(_wobbleTimer) * wobbleAmount;
        float wobbleRot = Mathf.Sin(_wobbleTimer * 0.7f) * rotationAmount;

        transform.position += new Vector3(wobbleX, wobbleY, 0);
        transform.rotation = Quaternion.Euler(0, 0, wobbleRot);

        lastTargetPosition = target.position;
    }


    public void SetEnabled(bool value)
    {
        _enabled = value;

        // »змен€ем размер камеры
        ppCamera.refResolutionX = 100;
        ppCamera.refResolutionY = 100;

        // —бросим дрожание при включении
        _wobbleTimer = 0f;
        transform.rotation = Quaternion.identity;
    }
}