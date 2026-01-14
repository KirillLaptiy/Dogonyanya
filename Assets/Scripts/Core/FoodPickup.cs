using System.Linq;
using Characters.Player;
using UnityEngine;

namespace Core
{
    public class FoodPickup : MonoBehaviour
    {
        private GameObject _player;
        private PlayerMovement _playerMovement;
        
        private void Start()
        {
            _player = FindNonBotPlayer();
            _playerMovement = _player.GetComponent<PlayerMovement>();
        }

        private void OnTriggerStay2D(Collider2D character)
        {
            if (!character.CompareTag(_player.tag)) return;
            if (!character.TryGetComponent(out PlayerStamina stamina)) return;
            if (!_playerMovement.IsRunning) return;

            stamina.AddStamina(20);
            Destroy(gameObject);
        }
        
        private GameObject FindNonBotPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            return players.FirstOrDefault(p => p != null && p.TryGetComponent<PlayerStamina>(out _));
        }
    }
}