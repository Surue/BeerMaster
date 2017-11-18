using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

    [Header("Game logic")]
    [SerializeField]
    private int health = 20;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void takeDamage(int damage) {
        health -= damage;

        if(health <= 0) {
            Debug.Log("Cette créature est morte");
            Destroy(this.gameObject);
        }
    }
}
