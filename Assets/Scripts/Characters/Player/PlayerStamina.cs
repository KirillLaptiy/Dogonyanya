using UnityEngine;
using UnityEngine.UI;

namespace Characters.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        public float maxStamina = 100f;
        public float regenPerSecond = 2f;

        [Header("Stamina Slider")]
        public float fadeSpeed = 5f;
        public float hideDelay = 0.5f;
        public Slider staminaSlider;
        public CanvasGroup staminaCanvasGroup;

        private PlayerMovement pm;
        private float stamina;

        private float hideTimer = 0f;
        private bool shouldShow = false;

        private void Awake()
        {
            pm = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            SetStamina(maxStamina);
        }

        private void Update()
        {
            UpdateStaminaBarUI();
        }

        private void FixedUpdate()
        {
            if (!pm.IsRunning)
            {
                float staminaToRegen = regenPerSecond;

                if (pm.IsWalking) staminaToRegen *= 0.5f;

                SetStamina(stamina + staminaToRegen * Time.deltaTime);
            }
        }

        public void SetStamina(float amount)
        {
            stamina = Mathf.Clamp(amount, 0, maxStamina);
            staminaSlider.value = stamina;

            if (stamina >= 99)
            {
                hideTimer += Time.deltaTime;
                if (hideTimer >= hideDelay)
                    shouldShow = false;
            }

            else
            {
                hideTimer = 0f;
                shouldShow = true;
            }
        }

        public void AddStamina(float amount)
        {
            SetStamina(stamina + amount);
        }

        public float GetStamina()
        {
            return stamina;
        }

        private void UpdateStaminaBarUI () {
            float targetAlpha = shouldShow ? 1f : 0f;

            staminaCanvasGroup.alpha = Mathf.MoveTowards(
                staminaCanvasGroup.alpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );
        }
    }
}
