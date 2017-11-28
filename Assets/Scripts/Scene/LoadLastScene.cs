using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLastScene : MonoBehaviour {

    private GameManager gameManager;

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown) {
            gameManager.LoadLastScene();
        }
	}
}
