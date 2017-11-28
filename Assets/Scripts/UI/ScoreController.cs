using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    [SerializeField]
    Text scoreText;

    InfoPlayer infoPlayer;

    int currentScore = 0;

	// Use this for initialization
	void Start () {
        infoPlayer = GameObject.FindObjectOfType<InfoPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentScore < infoPlayer.GetScore()) {
            currentScore++;
            scoreText.text = currentScore.ToString();
        }
	}
}
