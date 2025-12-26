using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Enemies.Parasite
{
    public class FoodSearch : AIStateBase
    {
        [Header("Food Settings")] public float waitTimeOnFood = 1.5f;
        public LayerMask foodLayer;

        [Header("Food eat progress Slider")] public float fadeSpeed = 5f;
        public float hideDelay = 0.5f;
        public Slider eatSlider;
        public CanvasGroup eatCanvasGroup;

        private Vector3Int _targetCell;
        private bool _hasTarget;
        private float _waitTimer = 0f;
        private bool _waitingOnFood = false;

        private AIController _aiController;
        private ParasitVision _parasiteVision;

        public override void OnStateEnter()
        {
            _aiController = GetComponent<AIController>();
            _parasiteVision = GetComponent<ParasitVision>();
            FindNewTarget();
            _waitingOnFood = false;
            _waitTimer = 0f;
        }

        public override void Tick()
        {
            if (!_hasTarget)
                FindNewTarget();

            // Если ждём на еде
            if (_waitingOnFood)
            {
                _waitTimer -= Time.deltaTime;
                eatSlider.value = _waitTimer;

                eatCanvasGroup.alpha = _waitTimer <= 0f ? 0 : 1;

                // Если паразит заметил цель, прерываем ожидание
                if (CanSeeThreat())
                {
                    _waitingOnFood = false;
                    _waitTimer = 0f;
                    _aiController.SetAIState(AIState.ParasitFlee);
                    return;
                }

                if (_waitTimer <= 0f)
                {
                    // После ожидания — удаляем еду
                    RemoveFoodAtCell(_targetCell);

                    _waitingOnFood = false;
                    _hasTarget = false;
                }

                return; // не двигаемся пока ждём
            }

            // Двигаемся по пути
            if (path != null && currentNode < path.Count)
            {
                MoveAlongPath();

                // Если дошли до цели
                if (currentNode >= path.Count)
                {
                    if (HasFood(_targetCell))
                    {
                        _waitingOnFood = true;
                        _waitTimer = waitTimeOnFood;
                    }
                    else
                    {
                        _hasTarget = false;
                    }
                }
            }
        }

        private void FindNewTarget()
        {
            Vector3Int startCell = WorldToCell(transform.position);

            List<Vector3Int> foodCells = FindAllFood();

            if (foodCells.Count > 0)
            {
                // Сортируем по ближайшей
                foodCells.Sort((a, b) =>
                {
                    float da = Vector3.Distance(CellToWorld(a), transform.position);
                    float db = Vector3.Distance(CellToWorld(b), transform.position);
                    return da.CompareTo(db);
                });

                foreach (var cell in foodCells)
                {
                    var testPath = FindPath(startCell, cell);
                    if (testPath != null && testPath.Count > 0)
                    {
                        _targetCell = cell;
                        path = testPath;
                        currentNode = 0;
                        _hasTarget = true;
                        return;
                    }
                }
            }

            // Если еды нет — гуляем случайно
            Vector3Int randomCell = FindRandomReachableCell(startCell);
            path = FindPath(startCell, randomCell);
            currentNode = 0;
            _hasTarget = path != null && path.Count > 0;
        }

        Vector3Int FindRandomReachableCell(Vector3Int startCell, int radius = 10)
        {
            List<Vector3Int> candidates = new();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Vector3Int cell = new(startCell.x + x, startCell.y + y, 0);

                    if (!groundTilemap.HasTile(cell)) continue;
                    if (obstacleTilemap.HasTile(cell)) continue;

                    candidates.Add(cell);
                }
            }

            Shuffle(candidates);

            foreach (var cell in candidates)
            {
                List<Vector3> testPath = FindPath(startCell, cell);

                if (testPath != null && testPath.Count > 0)
                    return cell;
            }

            return startCell; // fallback (на месте)
        }

        void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rnd = Random.Range(i, list.Count);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }

        List<Vector3Int> FindAllFood()
        {
            List<Vector3Int> foodCells = new List<Vector3Int>();

            foreach (var pos in groundTilemap.cellBounds.allPositionsWithin)
            {
                if (!groundTilemap.HasTile(pos)) continue;
                if (obstacleTilemap.HasTile(pos)) continue;

                if (HasFood(pos)) foodCells.Add(pos);
            }

            return foodCells;
        }

        bool HasFood(Vector3Int cell)
        {
            Vector3 pos = CellToWorld(cell);
            return Physics2D.OverlapCircle(pos, 0.2f, foodLayer);
        }

        void RemoveFoodAtCell(Vector3Int cell)
        {
            Vector3 pos = CellToWorld(cell);
            Collider2D food = Physics2D.OverlapCircle(pos, 0.2f, foodLayer);
            if (food != null)
                Destroy(food.gameObject);
        }

        bool CanSeeThreat()
        {
            // Здесь логика зрения паразита: видит игрока или союзника
            Transform seen;
            return _parasiteVision.CanSeeCharacter(true, out seen) ||
                   _parasiteVision.CanSeeCharacter(false, out seen);
        }

        Vector3 CellToWorld(Vector3Int cell)
        {
            return groundTilemap.GetCellCenterWorld(cell);
        }
    }
}