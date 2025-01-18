using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioClip fireSound;
    public AudioClip bombSound;
    public AudioClip hitSound;
    public AudioClip expSound;
    
    public AudioSource fxAudio;
    
    public AudioMixer audioMixer;

    private void Awake()
    {
        AudioSource[] temp = GetComponentsInChildren<AudioSource>();
        fxAudio = temp[1];
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
    
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}
