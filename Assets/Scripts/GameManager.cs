using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject deathScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayerDied()
    {
        Time.timeScale = 0f;          // ⏸ Пауза игры
        deathScreen.SetActive(true);  // 💀 Экран проигрыша
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
