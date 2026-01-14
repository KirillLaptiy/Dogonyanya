using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AkulaVision : MonoBehaviour
{
    [Header("Vision Settings")] public float viewWidth = 15f;
    public float viewHeight = 15f;
    public LayerMask obstacleLayer;
    public string enemyTag = "Enemy";

    private AIController aiController;
    // private AStarFoodPlace foodPlace;

    private Transform parasiteTransform;
    private AudioSource _audioSource;

    void Awake()
    {
        aiController = GetComponent<AIController>();
        _audioSource = GetComponent<AudioSource>();
        // foodPlace = GetComponent<AStarFoodPlace>();
    }

    private void Start()
    {
        var audioManager = AudioManager.Instance;
        audioManager.PlayAllySpawnSound(_audioSource);
    }

    void Update()
    {
        LookForCharacters();
    }

    void LookForCharacters()
    {
        // foodPlace.TryPlaceFood();

        bool seeEnemy = CanSeeEnemy(enemyTag);
        AIState currentState = aiController.GetAIState();

        if (seeEnemy)
        {
            // враг виден → бегаем
            if (currentState != AIState.Flee)
                aiController.SetAIState(AIState.Flee);
            return;
        }

        if (CanSeeParasite(out parasiteTransform))
        {
            if (currentState != AIState.AllyChase)
                aiController.SetAIState(AIState.AllyChase);
            return;
        }

        // враг не виден → сразу FoodPlace
        if (IsFarEnoughFromTarget() && currentState != AIState.FoodPlace)
        {
            aiController.SetAIState(AIState.FoodPlace);
        }
    }

    bool CanSeeEnemy(string characterTag)
    {
        Vector2 center = transform.position;
        GameObject[] players = GameObject.FindGameObjectsWithTag(characterTag);

        foreach (var player in players)
        {
            Vector2 playerPos = player.transform.position;

            if (Mathf.Abs(playerPos.x - center.x) > viewWidth / 2) continue;
            if (Mathf.Abs(playerPos.y - center.y) > viewHeight / 2) continue;

            Vector2 dir = (playerPos - center).normalized;
            float dist = Vector2.Distance(center, playerPos);

            if (!Physics2D.Raycast(center, dir, dist, obstacleLayer))
                return true;
        }

        return false;
    }

    public bool CanSeeParasite(out Transform target)
    {
        target = null;
        GameObject player = FindParasite();

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
            target = player.transform;
            return true;
        }

        return false;
    }

    GameObject FindParasite()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Parasite");
        return players.FirstOrDefault();
    }

    public Transform ParasiteTransform => parasiteTransform;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireCube(transform.position, new Vector3(viewWidth, viewHeight, 0f));
    }

    bool IsFarEnoughFromTarget()
    {
        if (parasiteTransform == null) return true;

        float dist = Vector3.Distance(transform.position, parasiteTransform.position);
        return dist >= (viewWidth + viewHeight) / 2 + 2f;
    }
}