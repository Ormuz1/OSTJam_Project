using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoundEffect : ScriptableObject
{
    public AudioClip audioClip;
    [Range(0, 1)] public float volume = 1;
}
