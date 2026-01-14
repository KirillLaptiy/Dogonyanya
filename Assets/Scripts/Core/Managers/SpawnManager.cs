using System.Collections;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Prefab A")] public GameObject prefabA;
        public float spawnDelayA = 90f;

        [Header("Prefab B")] public GameObject prefabB;
        public float spawnDelayB = 60f;

        private float timerA = 0f;
        private float timerB = 0f;

        private Transform[] spawnPoints;

        void Awake()
        {
            spawnPoints = GetComponentsInChildren<Transform>();
            spawnPoints = System.Array.FindAll(spawnPoints, t => t != transform);
        }

        void Update()
        {
            HandleSpawn(
                prefabA,
                spawnDelayA,
                ref timerA
            );

            HandleSpawn(
                prefabB,
                spawnDelayB,
                ref timerB
            );
        }

        void HandleSpawn(GameObject prefab, float delay, ref float timer)
        {
            if (IsTargetAlive(prefab))
            {
                timer = 0f; // 🔄 сброс таймера
                return;
            }

            timer += Time.deltaTime;

            if (timer >= delay)
            {
                Spawn(prefab);
                timer = 0f;
            }
        }

        bool IsTargetAlive(GameObject prefab)
        {
            string prefabTag = prefab.tag;

            if (prefabTag == "Player" && FindAlly())
                return true;

            return prefabTag != "Player" && GameObject.FindGameObjectWithTag(prefabTag) != null;
        }

        void Spawn(GameObject prefab)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(prefab, point.position, Quaternion.identity);
        }
        
        GameObject FindAlly()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            return players.FirstOrDefault(p => p != null && p.TryGetComponent<AkulaVision>(out _));
        }
    }
}