using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //Variable use when the player die to load last scene

    private static int indexLastScene;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    //Load next Scene, all level are in order and the WinScreen is the last one in the build
    public void NextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartGame() {
        SceneManager.LoadScene("level00");
        if(FindObjectOfType<InfoPlayer>() != null) {
            FindObjectOfType<InfoPlayer>().ResetScore();
        }
    }

    //When you die, load this screen
    public void Death() {
        indexLastScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("deadScreen");
    }

    public void LoadLastScene() {
        SceneManager.LoadScene(indexLastScene);
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit() {
        Application.Quit();
    }

    public bool IsMenuScene() {
        return SceneManager.GetActiveScene().name == "Menu";
    }

    public bool IsDeadScreenScene() {
        return SceneManager.GetActiveScene().name == "DeadScreen";
    }
}
