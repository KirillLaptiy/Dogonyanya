using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core
{
    public class TilemapManager : MonoBehaviour
    {
        public static TilemapManager Instance;

        public Tilemap groundTilemap;
        public Tilemap obstacleTilemap;

        private void Awake()
        {
            Instance = this;
        }
    }
}