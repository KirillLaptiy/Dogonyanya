using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    
    [Header("Music")]
    [SerializeField] private AudioClip[] tracks;
    
    private AudioSource _audioSource;
    
    private int _lastTrackIndex = -1;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    private void Start()
    {
        PlayRandomTrack();
    }

    private void Update()
    {
        // Если трек закончился — включаем следующий
        if (!_audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (tracks.Length == 0) return;

        int newIndex;

        // Гарантируем, что трек не повторится
        do
        {
            newIndex = Random.Range(0, tracks.Length);
        }
        while (newIndex == _lastTrackIndex && tracks.Length > 1);

        _lastTrackIndex = newIndex;

        _audioSource.clip = tracks[newIndex];
        _audioSource.Play();
    }
}