using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Physics")]
    [SerializeField]
    private float speed;
    [Header("Game logic")]
    [SerializeField]
    private int health;
    [SerializeField]
    private float swordRange = 0.5f;
    [SerializeField]
    private int attackPoint;
    [SerializeField]
    private GameObject sightPoint;

    private Rigidbody2D rigid;
    private Animator animatorController;

    private int maxHealth;
    private HealthBarController healthBarController;
    private int treasureValue = 0;

    private GameManager gameManager;

    private KeyController keyController;

    //Variable for attack with sword
    private bool attackWithSword = false;
    private float timeBetweenAttack = 0.375f;
    private float attackTimer = 0.0f;
    private bool isAttackingAnimation = false;

    private enum Direction {
        LEFT,
        RIGHT,
        TOP, 
        BOTTOM
    }
    
    private Direction direction;

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        rigid = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();

        healthBarController = GetComponent<HealthBarController>();
        if(healthBarController == null) {
            Debug.LogError("A health bar is missing");
        }
        healthBarController.SetMaxHealth(health);

        maxHealth = health;

        keyController = GetComponent<KeyController>();
        if(keyController == null)
        {
            Debug.LogError("A key controller is missing");
        }
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

        //Player Looking at cursor and select if using mouse position or joystick 
        Vector2 pos;
        if(IsThereJoystick()) {
            pos = new Vector3(Input.GetAxis("3rd Axis"), Input.GetAxis("4th Axis") * ( -1 ), 0);
            Debug.Log(Input.GetAxis("3rd Axis")+" "+Input.GetAxis("4th Axis"));
            if((Input.GetAxis("3rd Axis") <= 0.05 && Input.GetAxis("3rd Axis") >= -0.05)) {
                Debug.Log("Change x");
                pos.x = horizontalInput;
            }
                
            if ((Input.GetAxis("4th Axis") * ( -1 ) <= 0.05 && Input.GetAxis("4th Axis") * ( -1 ) >= -0.05 )) {
                Debug.Log("Change y");
                pos.y = verticalInput;
            }
        } else {
            pos = ( Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position ).normalized;
        }
        sightPoint.transform.rotation = Quaternion.LookRotation(pos);
        SetDirection();

        //Player attacke with sword
        if(Input.GetButtonDown("Fire1") && !isAttackingAnimation) {
            attackWithSword = true;
        }

        //Manage animation
        animatorController.SetFloat("speed", Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput)));
        ManageAnimation();

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

    void ManageAnimation() {
        animatorController.SetBool("lookingTop", false);
        animatorController.SetBool("lookingBottom", false);
        animatorController.SetBool("lookingLeft", false);
        animatorController.SetBool("lookingRight", false);

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
    }

    //Call when player attack, if a monster is detected then it take damage
    void CheckEnemiesTouched() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + sightPoint.transform.forward, swordRange, 1 << LayerMask.NameToLayer("Enemies"));
        List<GameObject> enemies = new List<GameObject>();
        
        //Check all collider and ignore if already in the list
        foreach(Collider2D collider in colliders) {
            if(!enemies.Contains(collider.gameObject)) {
                enemies.Add(collider.gameObject);
            }
        }

        foreach(GameObject enemi in enemies) {
            if(enemi.tag == "Spider") {
                enemi.GetComponent<SpiderController>().TakeDamage(attackPoint);
            }
            
            if(enemi.tag == "Bat") {
                enemi.GetComponent<BatController>().TakeDamage(attackPoint);
            }
        }
    }

    public bool IsLookingAt(GameObject gameObject) {
        return Physics2D.Raycast(transform.position, sightPoint.transform.forward, 1, 1 << LayerMask.NameToLayer("Item"));
    }

    public void TakeDamage(int damage) {
        health -= damage;

        if(health <= 0) {
            gameManager.Death();
            Destroy(this.gameObject);
        }

        healthBarController.UpdateHealthBar(health);
    }

    public void AddToTreasure(int value) {
        treasureValue += value;
    }

    void SetDirection() {
        if(Mathf.Abs(sightPoint.transform.forward.x) > Mathf.Abs(sightPoint.transform.forward.y)) {
            if(sightPoint.transform.forward.x > 0) {
                direction = Direction.RIGHT;
            } else {
                direction = Direction.LEFT;
            }
        } else {
            if(sightPoint.transform.forward.y > 0) {
                direction = Direction.TOP;
            } else {
                direction = Direction.BOTTOM;
            }
        }
    }

    //Use in Degug to draw in gizmos
    private void OnDrawGizmos(){
        //Debug affichage zone attaque épée
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position + sightPoint.transform.forward/3, swordRange);
    }

    //Search a joystick, if yes => true, else => false
    bool IsThereJoystick() {
        for(int i = 0; i < Input.GetJoystickNames().Length; i++) {
            if(Input.GetJoystickNames()[i] != null && Input.GetJoystickNames()[i] != "") {
                return true;
            }
        }

        return false;
    }
}
