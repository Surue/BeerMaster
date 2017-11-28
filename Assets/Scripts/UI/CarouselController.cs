using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarouselController : MonoBehaviour {

    [SerializeField]
    Image[] numberImages;
    [SerializeField]
    float timer;
    [SerializeField]
    float maxSpeed;

    float currentTimer = 0.0f;

    int finalNumber;
    bool isRunning = false;

    float currentSpeed = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(isRunning) {
            if(currentTimer >= timer) {
                isRunning = false;
            }
        }
	}

    public void LaunchAnimation() {
        isRunning = true;
    }

    public void SetNumberToGet(int number) {
        finalNumber = number;
        currentSpeed = 0.0f;
        currentTimer = 0.0f;
    }
}
