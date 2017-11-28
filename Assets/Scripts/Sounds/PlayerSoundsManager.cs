using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundsManager : MonoBehaviour {
    [SerializeField]
    AudioClip[] armorSounds;
    [SerializeField]
    AudioClip[] swordSounds;
    [SerializeField]
    AudioClip[] stepSounds;

    AudioSource[] audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponents<AudioSource>();
	}

    public void ArmorSound() {
        int indexSoundRandom = Random.Range(0, armorSounds.Length);
        if(!audioSource[0].isPlaying) {
            audioSource[0].clip = armorSounds[indexSoundRandom];
            audioSource[0].Play();
        }
    }

    public void SwordSound() {
        int indexSoundRandom = Random.Range(0, swordSounds.Length);
        audioSource[1].clip = swordSounds[indexSoundRandom];
        audioSource[1].Play();
    }

    public void StepSound() {
        int indexSoundRandom = Random.Range(0, stepSounds.Length);
        if(!audioSource[2].isPlaying) {
            audioSource[2].clip = stepSounds[indexSoundRandom];
            audioSource[2].Play();
        }
    }
}
