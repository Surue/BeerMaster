using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironnementSoundManager : MonoBehaviour {

    [SerializeField]
    AudioClip[] dropletSounds;
    [SerializeField]
    AudioSource audioSourceDroplet;
    [SerializeField]
    AudioClip[] dungeonSounds;
    [SerializeField]
    AudioSource audioSourceDungeon;

    const int minSecondsBetweenDropletSound = 10;
    const int maxSecondsBetweenDropletSound = 40;

    const int minSecondsBetweenDungeonSound = 30;
    const int maxSecondsBetweenDungeonSound = 70;

    // Use this for initialization
    void Start () {
        StartCoroutine(RandomDropletSounds());
        StartCoroutine(RandomDungeonSounds());
    }
	
	IEnumerator RandomDropletSounds() {
        while(true) {
            float seconds = Random.Range(minSecondsBetweenDropletSound, maxSecondsBetweenDropletSound) / 10;

            yield return new WaitForSeconds(seconds);

            int indexSoundRandom = Random.Range(0, dropletSounds.Length);

            if(!audioSourceDroplet.isPlaying) {
                audioSourceDroplet.clip = dropletSounds[indexSoundRandom];
                audioSourceDroplet.Play();
            }

        }
    }

    IEnumerator RandomDungeonSounds() {
        while(true) {
            float seconds = Random.Range(minSecondsBetweenDungeonSound, maxSecondsBetweenDungeonSound) / 10;

            yield return new WaitForSeconds(seconds);

            int indexSoundRandom = Random.Range(0, dungeonSounds.Length);

            if(!audioSourceDungeon.isPlaying) {
                audioSourceDungeon.clip = dungeonSounds[indexSoundRandom];
                audioSourceDungeon.Play();
            }

        }
    }
}
