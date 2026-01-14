using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject settingsPanel;
    public AudioMixer audioMixer;
    public AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip deathClip;

    private bool _settingOpened;
    private string exposedParam = "MyExposedParam";
    private string sfxParam = "SFXParam";
    private bool _gameEnded = false;
    [SerializeField] private CameraFollowDynamic cameraFollow;
    [SerializeField] private GameObject staminaSlider;
    [SerializeField] private GameObject healthBar;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.5f));
    }
    
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleSettings();
        }
    }

    public void PlayerDied()
    {
        audioSource.clip = deathClip;
        audioSource.Play();
        _gameEnded = true;
        Time.timeScale = 0f;
        deathScreen.SetActive(true);
    }
    
    public void PlayerWon()
    {
        audioSource.clip = winClip;
        audioSource.Play();
        _gameEnded = true;
        Time.timeScale = 0f;
        winScreen.SetActive(true);
    }
    
    public void ToggleSettings()
    {
        if (_gameEnded) return;
        
        _settingOpened = !_settingOpened;
        Time.timeScale = _settingOpened ? 0f : 1f;
        settingsPanel.SetActive(_settingOpened);
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
    
    public void SetMusicVolume(float value)
    {
        // Конвертируем 0–1 в дБ (-80…0)
        float dB;
        if (value <= 0.0001f)
            dB = -80f; // тишина
        else
            dB = Mathf.Log10(value) * 20f;

        audioMixer.SetFloat(exposedParam, dB);
        
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    
    public void SetSFXVolume(float value)
    {
        // Конвертируем 0–1 в дБ (-80…0)
        float dB;
        if (value <= 0.0001f)
            dB = -80f; // тишина
        else
            dB = Mathf.Log10(value) * 20f;

        audioMixer.SetFloat(sfxParam, dB);
        
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    
    public void ToggleDrunkMode(GameObject toggleObject)
    {
        var toggle = toggleObject.GetComponentInChildren<Toggle>();
        var text = toggleObject.GetComponentInChildren<TMP_Text>();
        
        bool value = toggle.isOn;
        toggle.interactable = false;
        cameraFollow.SetEnabled(value);
        ToggleSettings();

        text.SetText("С кайфом!");
        
        staminaSlider.SetActive(false);
        healthBar.SetActive(false);
    }
}
