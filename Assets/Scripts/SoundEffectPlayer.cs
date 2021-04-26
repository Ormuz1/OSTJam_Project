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

    public SoundEffect PlayRandom(SoundEffect[] soundEffects)
    {
        SoundEffect selectedSfx = soundEffects[Random.Range(0, soundEffects.Length)];
        Play(selectedSfx);
        return selectedSfx;
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

    public IEnumerator PlayRepeatedly(SoundEffect[] soundEffects, float interval, float duration = Mathf.Infinity)
    {
        SoundEffect chosenEffect;
        for(float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            chosenEffect = PlayRandom(soundEffects);
            yield return new WaitForSeconds(chosenEffect.audioClip.length + (interval - chosenEffect.audioClip.length));
        }
    }
}