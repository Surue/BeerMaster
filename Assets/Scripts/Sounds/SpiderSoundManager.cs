using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSoundManager : MonoBehaviour {

    [SerializeField]
    AudioClip[] chatteringSounds;
    [SerializeField]
    AudioClip[] dieSounds;

    AudioSource audioSource;

    float timeBetweenChatteringSound = 0.5f;
    float currentTimer;
    bool chatteringSoundStarted = false;

    // Use this for initialization
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if(chatteringSoundStarted) {
            currentTimer += Time.deltaTime;
            if(currentTimer >= timeBetweenChatteringSound) {
                currentTimer = 0.0f;
                chatteringSoundStarted = false;
            }
        }
    }

    public void ChatteringSound() {
        int indexSoundRandom = Random.Range(0, chatteringSounds.Length);

        if(!audioSource.isPlaying && !chatteringSoundStarted) {
            chatteringSoundStarted = true;
            audioSource.clip = chatteringSounds[indexSoundRandom];
            audioSource.Play();
        }
    }

    public void DieSound() {
        int indexSoundRandom = Random.Range(0, dieSounds.Length);

        if(!audioSource.isPlaying) {
            audioSource.clip = dieSounds[indexSoundRandom];
            audioSource.Play();
        }
    }

    public bool FinishDieSound() {
        return !audioSource.isPlaying;
    }
}
