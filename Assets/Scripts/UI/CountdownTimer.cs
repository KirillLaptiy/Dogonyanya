using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float duration = 300f;
    
    private GameManager _gameManager;

    void Start()
    {
        StartCoroutine(Timer());
        _gameManager = GameManager.Instance;
    }

    IEnumerator Timer()
    {
        float timeLeft = duration;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            UpdateText(timeLeft);
            yield return null;
        }

        UpdateText(0);
        OnTimerFinished();
    }
    
    void UpdateText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    void OnTimerFinished()
    {
        _gameManager.PlayerWon();
    }
}