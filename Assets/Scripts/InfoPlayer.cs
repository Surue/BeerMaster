using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPlayer : MonoBehaviour {

    private static GameObject lastObject = null;

    private int score;

    // Use this for initialization
    void Start () {
        if(lastObject == null) {
            DontDestroyOnLoad(gameObject);
            lastObject = gameObject;
        } else {
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddScore(int value) {
        score += value;
    }

    public float GetScore() {
        return score;
    }
}
