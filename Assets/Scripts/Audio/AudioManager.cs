using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip brickSound;
    [SerializeField] private AudioClip rowClearSound;

    public void PlayBrickSound(AudioSource audioSource)
    {
        audioSource.clip = brickSound;
        audioSource.Play();
    }

    public void PlayRowClearSound(AudioSource audioSource)
    {
        audioSource.clip = rowClearSound;
        audioSource.Play();
    }
}
