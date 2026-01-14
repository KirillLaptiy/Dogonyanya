using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip hitClip;
    public AudioClip parasiteSpawnClip;
    public AudioClip allySpawnClip;
    
    private void Awake()
    {
        Instance = this;
    }

    public void PlayHitSound(AudioSource source)
    {
        source.clip = hitClip;
        source.Play();
    }
    
    public void PlayParasiteSpawnSound(AudioSource source)
    {
        source.clip = parasiteSpawnClip;
        source.Play();
    }
    
    public void PlayAllySpawnSound(AudioSource source)
    {
        source.clip = allySpawnClip;
        source.Play();
    }
    
    public void PlayTemporaryHitSound(AudioSource source)
    {
        AudioSource.PlayClipAtPoint(
            hitClip,
            source.transform.position,
            source.volume
        );
    }
}
