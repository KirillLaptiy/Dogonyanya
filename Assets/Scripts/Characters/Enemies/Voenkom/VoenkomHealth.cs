using UnityEngine;

public class VoenkomHealth : MonoBehaviour
{
    public float stunTime = 2f;

    private bool isStunned = false;
    private float stunEndTime = 0f;

    private VoenkomHitCharacter vhc;
    private AIController voenkomController;
    private VoenkomVision vision;

    private void Awake()
    {
        vhc = GetComponent<VoenkomHitCharacter>();
        voenkomController = GetComponent<AIController>();
        vision = GetComponent<VoenkomVision>();
    }

    void Update()
    {
        if (!isStunned) return;

        if (Time.time >= stunEndTime)
        {
            isStunned = false;
            ClearStunEffects();
        }
    }

    public void ApplyStun(float stunAdditionalDuration = 0f)
    {
        isStunned = true;
        stunEndTime = Time.time + stunTime + stunAdditionalDuration;
        ApplyStunEffects();
    }

    public bool IsStunned => isStunned;

    private void ApplyStunEffects() {
        vhc.SetStunned(true);
        voenkomController.SetStunned(true);
        voenkomController.SetAIState(AIState.Search);
        vision.SetStunned(true);
    }

    private void ClearStunEffects()
    {
        vhc.SetStunned(false);
        voenkomController.SetStunned(false);
        vision.SetStunned(false);
    }
}
