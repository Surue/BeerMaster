using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [Header("Physics")]
    [SerializeField]
    private float speed;
    [Header("UI")]
    [SerializeField]
    private Image healthBar;
    [Header("Game logic")]
    [SerializeField]
    private int health;
    [SerializeField]
    private float swordRange = 0.5f;
    [SerializeField]
    private float attackPoint;

    private Rigidbody2D rigid;
    private Animator animatorController;

    private int maxHealth;
    private int treasureValue = 0;

    //Variable for attack with sword
    private bool attackWithSword = false;
    private float timeBetweenAttack = 0.375f;
    private float attackTimer = 0.0f;
    private bool isAttackingAnimation = false;

    private Vector2 attackDirection; //TO DELETE FOR FINAL PROJECT

    private enum Direction {
        LEFT,
        RIGHT,
        TOP, 
        BOTTOM
    }

    private Direction direction;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();

        maxHealth = health;
	}
	
	// Update is called once per frame
	void Update () {
        //Player Mouvement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if(horizontalInput == 0) {
            horizontalInput *= rigid.velocity.x * ( -1 );
        } else {
            horizontalInput *= speed;
        }

        if(verticalInput == 0) {
            verticalInput *= rigid.velocity.y * ( -1 );
        } else {
            verticalInput *= speed;
        }

        rigid.velocity = new Vector2(horizontalInput, verticalInput);

        //Player Looking at cursor
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var diffX = rigid.position.x - mouse.x;
        var diffY = rigid.position.y - mouse.y;

        if(Mathf.Abs(diffX) > Mathf.Abs(diffY)) {
            if(diffX < 0) {
                direction = Direction.RIGHT;
            } else {
                direction = Direction.LEFT;
            }
        } else {
            if(diffY < 0) {
                direction = Direction.TOP;
            } else {
                direction = Direction.BOTTOM;
            }
        }

        //Construct attack direction vector
        attackDirection = ( rigid.position - mouse ).normalized;

        //Player attacke with sword
        if(Input.GetButtonDown("Fire1") && !isAttackingAnimation) {
            attackWithSword = true;
        }

        //Manage animation
        animatorController.SetBool("lookingTop", false);
        animatorController.SetBool("lookingBottom", false);
        animatorController.SetBool("lookingLeft", false);
        animatorController.SetBool("lookingRight", false);

        animatorController.SetFloat("speed", Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput)));

        switch(direction) {
            case Direction.LEFT:
                animatorController.SetBool("lookingLeft", true);
                if(attackWithSword) {
                    animatorController.SetTrigger("attackSwordLeft");
                    CheckEnemiesTouched();
                }
                break;

            case Direction.RIGHT:
                animatorController.SetBool("lookingRight", true);
                if(attackWithSword) {
                    animatorController.SetTrigger("attackSwordRight");
                    CheckEnemiesTouched();
                }
                break;

            case Direction.TOP:
                animatorController.SetBool("lookingTop", true);
                if(attackWithSword) {
                    animatorController.SetTrigger("attackSwordTop");
                    CheckEnemiesTouched();
                }
                break;

            case Direction.BOTTOM:
                animatorController.SetBool("lookingBottom", true);
                if(attackWithSword) {
                    animatorController.SetTrigger("attackSwordBottom");
                    CheckEnemiesTouched();
                }
                break;

            default:
                break;
        }

        //Check if can attack
        if(attackWithSword) {
            attackWithSword = false;
            isAttackingAnimation = true;
        }

        if(isAttackingAnimation) {
            attackTimer += Time.deltaTime;
            if(attackTimer >= timeBetweenAttack) {
                isAttackingAnimation = false;
                attackTimer = 0.0f;
            }
        }
    }

    void CheckEnemiesTouched() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(rigid.position + ( attackDirection / -3 ), swordRange, 1 << LayerMask.NameToLayer("Enemies"));
        List<GameObject> enemies = new List<GameObject>();

        //Check all collider and ignore if already in the list
        foreach(Collider2D collider in colliders) {
            if(!enemies.Contains(collider.gameObject)) {
                enemies.Add(collider.gameObject);
            }
        }

        foreach(GameObject enemi in enemies) {
            enemi.SendMessage("TakeDamage", attackPoint, SendMessageOptions.DontRequireReceiver);
        }
    }

    void TakeDamage(int damage) {
        health -= damage;

        if(health <= 0) {
            Destroy(this.gameObject);
        }

        healthBar.fillAmount = 1 - ( ( maxHealth - health ) / (float)maxHealth );
    }

    public void AddToTreasure(int value) {
        treasureValue += value;
    }

    private void OnDrawGizmos(){
        //Debug affichage zone attaque épée
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(rigid.position + (attackDirection / -3), swordRange);
    }
}
