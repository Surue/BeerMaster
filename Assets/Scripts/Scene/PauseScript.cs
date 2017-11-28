using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseScript : MonoBehaviour {

    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject uiGamePanel;

    private bool isPaused = false;

    private static PauseScript instance;

    public static PauseScript Instance {
        get {
            return instance;
        }
    }

    public void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetButton("Pause") && !isPaused) {
            isPaused = true;
            pausePanel.SetActive(true);
            uiGamePanel.SetActive(false);
            Time.timeScale = 0;
        }
    }

    public void OnResumeGameButtonDown() {
        isPaused = false;
        pausePanel.SetActive(false);
        uiGamePanel.SetActive(true);
        Time.timeScale = 1;
    }
}

