using UnityEngine;

public enum AIState
{
    Chase,
    Flee,
    Search,
}

class AIController : MonoBehaviour
{
    [Header("States")]
    public AStarChase chase;
    public AStarFlee flee;
    public AStarSearch search;

    private AIState currentStateEnum = AIState.Search;

    private AIStateBase lastActiveState;

    void Update()
    {
        // Выключаем все состояния
        if (chase != null) chase.enabled = false;
        if (flee != null) flee.enabled = false;
        if (search != null) search.enabled = false;


        AIStateBase activeState = null;
        switch (currentStateEnum)
        {
            case AIState.Chase: activeState = chase; break;
            case AIState.Flee: activeState = flee; break;
            case AIState.Search: activeState = search; break;
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
}
