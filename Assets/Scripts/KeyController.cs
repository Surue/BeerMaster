using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyController : MonoBehaviour {

    [SerializeField]
    private Text keyNumberText;

    private int keyInInventory = 0;

    // Use this for initialization
    void Start () {
        DisplayKeyNumber();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddKey()
    {
        keyInInventory++;
        DisplayKeyNumber();
    }

    public void UseKey()
    {
        keyInInventory--;
        DisplayKeyNumber();
    }

    public bool HasKey()
    {
        return keyInInventory > 0;
    }

    void DisplayKeyNumber()
    {
        if (keyInInventory == 0)
        {
            keyNumberText.text = "";
        }
        else
        {
            keyNumberText.text = keyInInventory.ToString();
        }
    }
}
