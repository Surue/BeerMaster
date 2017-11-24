using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableController : MonoBehaviour {

    [SerializeField]
    private int value;
    [SerializeField]
    private TypeObject typeObject;

    private PlayerController player;
    private KeyController playerKeyController;

    private enum TypeObject {
        TREASURE,
        KEY
    }

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerController>();
        playerKeyController = player.GetComponent<KeyController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {

            GiveObjectToPlayer();
            Destroy(this.gameObject);
        }
    }

    void GiveObjectToPlayer() {
        switch(typeObject) {
            case TypeObject.TREASURE:
                player.AddToTreasure(value);
                break;

            case TypeObject.KEY:
                playerKeyController.AddKey();
                break;
        }
    }
}
