using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatController : MonoBehaviour {

    [Header("Physics")]
    [SerializeField]
    private float speed;
    [Header("Game logic")]
    [SerializeField]
    private int health = 20;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private GameObject sightPoint;
    [SerializeField]
    private int attackPoint = 1;

    private int maxHealth;
    private Rigidbody2D rigid;
    private Animator animatorController;
    private Vector3 destination;

    //Timer for IA
    private float currentTimer = 0.0f;
    private float idleTimer = 2.0f;
    private float attackTimer = .5f;

    //Variable for IA
    private bool playerFound;


    private enum Direction {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    private enum State {
        IDLE,
        MOVING,
        ATTACKING
    }

    private Direction direction;
    private State state = State.IDLE;

    // Use this for initialization
    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();
        maxHealth = health;
    }

    // Update is called once per frame
    void Update() {
        //Player Looking at cursor
        Vector3 pos = (destination - transform.position ).normalized;
        Quaternion rotation = Quaternion.LookRotation(pos);
        sightPoint.transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);

        SetDirection();

        //Detection of Player in sight
        CheckPlayerPresence();

        switch(state) {
            case State.IDLE:
                currentTimer += Time.deltaTime;

                if(playerFound) {
                    state = State.MOVING;
                    currentTimer = 0.0f;
                } else if(currentTimer >= idleTimer) {
                    currentTimer = 0.0f;

                    //Choose a destination
                    destination = RandomPoint();
                    state = State.MOVING;
                }
                break;

            case State.MOVING:
                if(Vector3.Distance(transform.position, destination) <= 0.1f) {
                    state = State.IDLE;
                } else {
                    rigid.velocity = (destination - transform.position ).normalized * speed;
                }

                if(CheckEnemiesTouched()) {
                    currentTimer = 0.0f;
                    state = State.ATTACKING;
                }

                break;

            case State.ATTACKING:
                rigid.velocity = new Vector2(0, 0);

                currentTimer += Time.deltaTime;
                if(currentTimer >= attackTimer) {
                    state = State.MOVING;
                }
                break;
        }

        //Manage animation
        ManageAnimation();
    }

    bool CheckEnemiesTouched() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + sightPoint.transform.forward / 3, 0.3f, 1 << LayerMask.NameToLayer("Player"));

        foreach(Collider2D collider in colliders) {
            collider.gameObject.SendMessage("TakeDamage", attackPoint, SendMessageOptions.DontRequireReceiver);
            return true;
        }

        return false;
    }

    private void OnDrawGizmos() {
        //Debug affichage zone attaque épée
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position + sightPoint.transform.forward/3, 0.3f);
    }

    Vector3 RandomPoint() {
        bool find = false;
        Vector3 tmpDestination = new Vector3();

        while(!find) {
            tmpDestination = Random.insideUnitCircle * 1;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, tmpDestination, 2, 1 << LayerMask.NameToLayer("Wall"));
            if(hit.collider == null) {
                find = true;
            } 
        }

        return tmpDestination;
    }

    void ManageAnimation() {
        animatorController.SetBool("lookingUp", false);
        animatorController.SetBool("lookingDown", false);
        animatorController.SetBool("lookingLeft", false);
        animatorController.SetBool("lookingRight", false);

        switch(direction) {
            case Direction.LEFT:
                animatorController.SetBool("lookingLeft", true);
                break;

            case Direction.RIGHT:
                animatorController.SetBool("lookingRight", true);
                break;

            case Direction.TOP:
                animatorController.SetBool("lookingUp", true);
                break;

            case Direction.BOTTOM:
                animatorController.SetBool("lookingDown", true);
                break;

            default:
                break;
        }
    }

    void TakeDamage(int damage) {
        health -= damage;

        if(health <= 0) {
            Destroy(this.gameObject);
        }

        healthBar.fillAmount = 1 - ( ( maxHealth - health ) / (float)maxHealth );
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

    void CheckPlayerPresence() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));

        if(hit.collider != null) {
            playerFound = true;
            destination = hit.transform.position;
        } else {
            playerFound = false;
        }
    }
}
