using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip[] weaponAudioClips;
    [SerializeField] private AudioClip reloadStartAudioClip;
    [SerializeField] private AudioClip reloadPerfectAudioClip;
    [SerializeField] private AudioClip reloadEndAudioClip;
    [SerializeField] private AudioClip[] footstepWalkAudioClips;
    [SerializeField] private AudioClip[] footstepSprintAudioClips;
    [SerializeField] private AudioClip[] pickupGatherAudioClips;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    private void PlayRandomAudio(AudioClip[] audioClips, float volume = 0.35f)
    {
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
    private void PlayRandomAudio(AudioClip audioClip, float volume = 0.35f)
    {
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(audioClip);
    }
    public void PlayRandomWeaponAudio(float volume = 0.35f) => PlayRandomAudio(weaponAudioClips, volume);
    public void PlayReloadStartAudio(float volume = 0.35f) => PlayRandomAudio(reloadStartAudioClip, volume);
    public void PlayReloadPerfectAudio(float volume = 0.35f) => PlayRandomAudio(reloadPerfectAudioClip, volume);
    public void PlayReloadEndAudio(float volume = 0.35f) => PlayRandomAudio(reloadEndAudioClip, volume);
    public void PlayFootstepWalkAudio(float volume = 0.35f) => PlayRandomAudio(footstepWalkAudioClips, volume);
    public void PlayFootstepSprintAudio(float volume = 0.35f) => PlayRandomAudio(footstepSprintAudioClips, volume);
    public void PlayPickupGatherAudio(float volume = 0.35f) => PlayRandomAudio(pickupGatherAudioClips, volume);
}
