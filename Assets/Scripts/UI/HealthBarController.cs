using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarController : MonoBehaviour {

    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image healthBarBackground;

    private int maxHealth;

    public void SetMaxHealth(int health) {
        maxHealth = health;
    }

    public void UpdateHealthBar(int health) {
        healthBar.fillAmount = 1 - ( ( maxHealth - health ) / (float)maxHealth );
    }

    public void HideHealthBar() {
        healthBar.enabled = false;
        healthBarBackground.enabled = false;
    }
}
