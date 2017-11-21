using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockableController : MonoBehaviour {

    [SerializeField]
    private Text textToDisplay;

    private PlayerController player;
    private TreasureController treasure; //Null if is not a treasure
    private Animator animatorController;

    private bool IsLocked = true;

    // Use this for initialization
    void Start () {
        animatorController = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();

        treasure = GetComponent<TreasureController>();
    }
	
	// Update is called once per frame
	void Update () {
        if(IsLocked) {
            bool hasPressedF = false;
            if(Input.GetKeyDown("f")) {
                hasPressedF = true;
            }

            textToDisplay.text = "";
            if(player.IsLookingAt(this.gameObject)) {

                if(player.HasKey()) {
                    textToDisplay.text = "Appuyer sur F pour ouvrir";
                    if(hasPressedF) {
                        textToDisplay.text = "";
                        animatorController.SetTrigger("open");
                        player.UseKey();

                        if(treasure != null) {
                            treasure.DropTreasure();
                        }
                        IsLocked = false;
                    }
                } else {
                    textToDisplay.text = "Il vous faut une clé pour ouvrir";
                }
            }
        }
    }
}
