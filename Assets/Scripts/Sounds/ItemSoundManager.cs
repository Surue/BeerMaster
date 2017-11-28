using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSoundManager : MonoBehaviour {

    public static ItemSoundManager Instance;

    [SerializeField]
    AudioClip[] coinSounds;
    [SerializeField]
    AudioClip[] doorSounds;
    [SerializeField]
    AudioClip[] chestSounds;
    [SerializeField]
    AudioClip[] woodHitSounds;
    [SerializeField]
    AudioSource audioSource;

    // Use this for initialization
    private void Awake() {
        if(Instance != null) {
            Debug.Log("Multiple instances of ItemSoundManager");
        }
        Instance = this;
    }

    void Update() {
    }

    public void CoinSound() {
        int indexSoundRandom = Random.Range(0, coinSounds.Length);

        if(!audioSource.isPlaying) {
            audioSource.clip = coinSounds[indexSoundRandom];
            audioSource.Play();
        }
    }

    public void DoorSound() {
        int indexSoundRandom = Random.Range(0, doorSounds.Length);

        if(!audioSource.isPlaying) {
            audioSource.clip = doorSounds[indexSoundRandom];
            audioSource.Play();
        }
    }

    public void WoodHitSound() {
        int indexSoundRandom = Random.Range(0, woodHitSounds.Length);

        if(!audioSource.isPlaying) {
            audioSource.clip = woodHitSounds[indexSoundRandom];
            audioSource.Play();
        }
    }
}
