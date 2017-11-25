using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinkingContronller : MonoBehaviour {
    [SerializeField]
    private float spriteBlinkingTotalDuration = 1.0f;

    private Text text;

    private float spriteBlinkingTimer = 0.0f;
    private float spriteBlinkingMiniDuration;

    // Use this for initialization
    void Start () {
        spriteBlinkingMiniDuration = spriteBlinkingTotalDuration / 2.0f;
        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration) {
            spriteBlinkingTimer = 0.0f;

            if (text.enabled == true) {
                text.enabled = false;
            }
            else {
                text.enabled = true;
            }
        }
    }
}
