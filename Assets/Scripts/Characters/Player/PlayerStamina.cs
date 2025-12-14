using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        SetStamina(maxStamina);
    }

    void Update()
    {
        float targetAlpha = shouldShow ? 1f : 0f;
        staminaCanvasGroup.alpha = Mathf.MoveTowards(
            staminaCanvasGroup.alpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );
    }

    private void FixedUpdate()
    {
        if (!pm.IsRunning)
        {
            float staminaToRegen = regenPerSecond;

            if (pm.IsWalking) staminaToRegen *= 0.5f;

            SetStamina(stamina + staminaToRegen * Time.deltaTime);
            print(stamina);
        }
    }

    public void SetStamina(float stamina)
    {
        this.stamina = Mathf.Clamp(stamina, 0, maxStamina);
        staminaSlider.value = this.stamina;

        if (this.stamina >= 99)
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

    public float GetStamina()
    {
        return stamina;
    }
}
