using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public bool isMusicSlider;
    
    private GameManager _gameManager;

    private void Awake()
    {
        slider.value = PlayerPrefs.GetFloat(isMusicSlider ? "MusicVolume" : "SFXVolume", 0.5f);
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        
        // Привязываем событие к слайдеру
        slider.onValueChanged.AddListener(isMusicSlider ? _gameManager.SetMusicVolume : _gameManager.SetSFXVolume);
    }
}