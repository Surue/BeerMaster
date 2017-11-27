using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    [SerializeField]
    Text scoreText;

    InfoPlayer infoPlayer;

	// Use this for initialization
	void Start () {
        infoPlayer = GameObject.FindObjectOfType<InfoPlayer>();
        scoreText.text = infoPlayer.GetScore().ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
