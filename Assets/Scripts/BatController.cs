using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatController : MonoBehaviour {

    [Header("Game logic")]
    [SerializeField]
    private int health = 20;
    [SerializeField]
    private Image healthBar;

    private int maxHealth;

    // Use this for initialization
    void Start () {
        maxHealth = health;
    }
	
	// Update is called once per frame
	void Update () {
	}

    void TakeDamage(int damage) {
        health -= damage;

        if(health <= 0) {
            Debug.Log("Cette créature est morte");
            Destroy(this.gameObject);
        }

        healthBar.fillAmount = 1 - ((maxHealth - health) / (float)maxHealth);
    }
}
