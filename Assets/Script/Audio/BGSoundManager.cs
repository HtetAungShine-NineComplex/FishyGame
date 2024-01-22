using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSoundManager : Singleton<BGSoundManager>
{
    [SerializeField] public AudioClip _loadingSound;
    [SerializeField] public AudioClip _mainMenuSound;
    [SerializeField] public AudioClip _gameplaySound;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        PlaySound(_loadingSound);
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.Stop();

        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
