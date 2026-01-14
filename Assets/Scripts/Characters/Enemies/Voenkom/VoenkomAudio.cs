using System.Collections;
using UnityEngine;

public class VoenkomAudio : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private float secondsBetweenLoops = 35f;
    
    private AudioSource _audioSource;
    
    private int _lastTrackIndex = -1;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    private void Start()
    {
        StartCoroutine(PlaySoundLoop());
    }
    
    private IEnumerator PlaySoundLoop()
    {
        while (true)
        {
            PlayRandomSound();
            yield return new WaitForSeconds(secondsBetweenLoops);
        }
    }

    private void PlayRandomSound()
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

        _audioSource.PlayOneShot(tracks[newIndex]);
    }
}
