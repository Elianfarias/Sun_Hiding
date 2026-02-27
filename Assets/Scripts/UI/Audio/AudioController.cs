using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }
    [SerializeField] private AudioClip soundHover;
    [SerializeField] private AudioClip soundClick;
    [SerializeField] private AudioSource soundEffectAudioSource;
    [SerializeField] private AudioSource BackgroundAudioSource;
    [SerializeField] private AudioSource buttonsAudioSource;

    private int actualPriority = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySoundEffect(AudioClip audioClip, int priority = 0)
    {
        if (soundEffectAudioSource.isPlaying && actualPriority > priority)
            return;

        actualPriority = priority;
        soundEffectAudioSource.clip = audioClip;
        soundEffectAudioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        BackgroundAudioSource.Stop();
    }


    public void PlayBackgroundMusic()
    {
        if(!BackgroundAudioSource.isActiveAndEnabled)
            BackgroundAudioSource.Play();
    }
    
    public void PlayButtonClickSound()
    {
        buttonsAudioSource.PlayOneShot(soundClick);
    }

    public void PlayButtonHoverSound()
    {
        buttonsAudioSource.PlayOneShot(soundHover);
    }
}
