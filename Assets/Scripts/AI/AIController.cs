using UnityEngine;
using Characters.Enemies.Parasite;

public enum AIState
{
    Chase,
    Flee,
    Search,
    FoodPlace,
    FoodSearch,
    ParasitFlee,
    AllyChase,
}

class AIController : MonoBehaviour
{
    [Header("States")]
    public AIState currentStateEnum = AIState.Search;
    public AStarChase chase;
    public AStarFlee flee;
    public AStarSearch search;
    public AStarFoodPlace foodPlace;
    public FoodSearch foodSearch;
    public ParasitFlee parasitFlee;
    public AllyChase allyChase;

    private AIStateBase lastActiveState;

    private bool isStunned;

    void Update()
    {
        if (isStunned) return;

        // ��������� ��� ���������
        if (chase != null) chase.enabled = false;
        if (flee != null) flee.enabled = false;
        if (search != null) search.enabled = false;
        if (foodPlace != null) foodPlace.enabled = false;
        if (foodSearch != null) foodSearch.enabled = false;
        if (parasitFlee != null) parasitFlee.enabled = false;
        if (allyChase != null) allyChase.enabled = false;


        AIStateBase activeState = null;
        switch (currentStateEnum)
        {
            case AIState.Chase: activeState = chase; break;
            case AIState.Flee: activeState = flee; break;
            case AIState.Search: activeState = search; break;
            case AIState.FoodPlace: activeState = foodPlace; break;
            case AIState.FoodSearch: activeState = foodSearch; break;
            case AIState.ParasitFlee: activeState = parasitFlee; break;
            case AIState.AllyChase: activeState = allyChase; break;
        }

        if (activeState)
        {
            activeState.enabled = true;

            if (activeState != lastActiveState)
            {
                activeState.OnStateEnter();
                lastActiveState = activeState;
            }

            activeState.Tick();
        }
    }

    public void SetAIState(AIState state)
    {
        currentStateEnum = state;
    }

    public AIState GetAIState()
    {
        return currentStateEnum;
    }

    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
    }
}
