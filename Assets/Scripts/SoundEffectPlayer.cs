using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource audioSource;


    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        if(!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    
    public void Play(SoundEffect soundEffect)
    {
        if(soundEffect != null)
            audioSource.PlayOneShot(soundEffect.audioClip, soundEffect.volume);
    }

    public void PlayRandom(SoundEffect[] soundEffects)
    {
        SoundEffect selectedSfx = soundEffects[Random.Range(0, soundEffects.Length)];
        Play(selectedSfx);
    }

    public IEnumerator PlayRepeatedly(SoundEffect soundEffect, float duration = Mathf.Infinity)
    {
        WaitForSeconds waitForClip = new WaitForSeconds(soundEffect.audioClip.length);
        for(float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            Play(soundEffect);
            yield return waitForClip;
        }
    }
}