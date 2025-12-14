using UnityEngine;

public enum AIState
{
    Chase,
    Flee,
    Search,
}

class AIController : MonoBehaviour
{
    public AIState currentStateEnum;

    [Header("States")]
    public AStarChase chase;
    public AStarFlee flee;
    public AStarSearch search;


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
            print(activeState);
            activeState.enabled = true;
            activeState.Tick();
        }
    }
}
