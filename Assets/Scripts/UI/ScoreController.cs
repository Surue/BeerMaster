using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    
    Text scoreText;

    InfoPlayer infoPlayer;

    int currentScore = 0;

    GameManager gameManager;

	// Use this for initialization
	void Start () {
        infoPlayer = GameObject.FindObjectOfType<InfoPlayer>();
        gameManager = FindObjectOfType<GameManager>();

        if(!gameManager.IsMenuScene() && !gameManager.IsDeadScreenScene()) {
            scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(scoreText.IsDestroyed() && (gameManager.IsMenuScene() || gameManager.IsDeadScreenScene())) {
            scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        }else if(currentScore < infoPlayer.GetScore()) {
            currentScore++;
            scoreText.text = currentScore.ToString();
        }
	}
}
