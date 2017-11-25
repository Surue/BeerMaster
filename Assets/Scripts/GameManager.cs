using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //Variable use when the player die to load last scene

    private static int indexLastScene;
	
    // Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Load next Scene, all level are in order and the WinScreen is the last one in the build
    public void NextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartGame() {
        SceneManager.LoadScene("Test_Level");
    }

    //When you die, load this screen
    public void Death() {
        indexLastScene = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("die");
        Debug.Log(indexLastScene);
        SceneManager.LoadScene("deadScreen");
    }

    public void LoadLastScene() {
        Debug.Log(indexLastScene);
        Debug.Log(SceneManager.sceneCount);
        SceneManager.LoadScene(indexLastScene);
    }
}
