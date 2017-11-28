using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSoundManager : MonoBehaviour {
    [SerializeField]
    AudioClip[] wingSounds;
    [SerializeField]
    AudioClip[] dieSounds;

    AudioSource audioSource;

    float timeBetweenWingSoung = 0.2f;
    float currentTimer;
    bool wingSoundStarted = false;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if(wingSoundStarted) {
            currentTimer += Time.deltaTime;
            if(currentTimer >= timeBetweenWingSoung) {
                currentTimer = 0.0f;
                wingSoundStarted = false;
            }
        }
    }

    public void WingSound() {
        int indexSoundRandom = Random.Range(0, wingSounds.Length);

        if(!audioSource.isPlaying && !wingSoundStarted) {
            wingSoundStarted = true;
            audioSource.clip = wingSounds[indexSoundRandom];
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
