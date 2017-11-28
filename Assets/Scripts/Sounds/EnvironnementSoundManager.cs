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

    // Use this for initialization
    void Start () {
        StartCoroutine(RandomDropletSounds());
        StartCoroutine(RandomDungeonSounds());
    }
	
	IEnumerator RandomDropletSounds() {
        while(true) {
            float seconds = Random.Range(10, 40) / 10;

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
            float seconds = Random.Range(30, 70) / 10;

            yield return new WaitForSeconds(seconds);

            int indexSoundRandom = Random.Range(0, dungeonSounds.Length);

            if(!audioSourceDungeon.isPlaying) {
                audioSourceDungeon.clip = dungeonSounds[indexSoundRandom];
                audioSourceDungeon.Play();
            }

        }
    }
}
