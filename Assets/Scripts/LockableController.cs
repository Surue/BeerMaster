using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockableController : MonoBehaviour {

    [SerializeField]
    private Text textToDisplay;
    [SerializeField]
    private Type type;

    private PlayerController player;
    private KeyController playerKeyController;
    private TreasureController treasure; //Null if is not a treasure
    private Animator animatorController;

    private bool IsLocked = true;

    private enum Type {
        CHEST,
        DOOR
    }

    // Use this for initialization
    void Start () {
        animatorController = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();
        playerKeyController = player.GetComponent<KeyController>();

        treasure = GetComponent<TreasureController>();
    }
	
	// Update is called once per frame
	void Update () {
        if(IsLocked) {
            bool hasPressedF = false;
            if(Input.GetButtonDown("Use")) {
                hasPressedF = true;
            }

            textToDisplay.text = "";
            if(player.IsLookingAt(this.gameObject)) {
                if(playerKeyController.HasKey()) {
                    textToDisplay.text = "Appuyer sur F ou L1 pour ouvrir";
                    if(hasPressedF) {
                        textToDisplay.text = "";
                        animatorController.SetTrigger("open");
                        playerKeyController.UseKey();

                        switch(type) {
                            case Type.CHEST:
                                treasure.DropTreasure();
                                IsLocked = false;
                                break;

                            //If it's a door => delete collider
                            case Type.DOOR:
                                GetComponent<Collider2D>().enabled = false;
                                IsLocked = false;
                                break;
                        }
                    }
                } else {
                    textToDisplay.text = "Il vous faut une clé pour ouvrir";
                }
            }
        }
    }
}
