using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatController : MonoBehaviour {

    [Header("Physics")]
    [SerializeField]
    private float speed = 1;
    [Header("Game logic")]
    [SerializeField]
    private int health = 20;
    [SerializeField]
    private GameObject sightPoint;
    [SerializeField]
    private int attackPoint = 1;
    [Header("Loot")]
    [SerializeField]
    private int maxValue;
    [SerializeField]
    private GameObject coinPrefab;
    
    private int maxHealth;
    private HealthBarController healthBarController;
    private Rigidbody2D rigid;
    private Animator animatorController;
    private Vector3 destination;

    //Variable for target
    private GameObject target;
    private float speedChasing;

    //Timer for IA
    private float currentTimer = 0.0f;
    private float idleTimer = 2.0f;
    private float attackTimer = .5f;
    
    private enum Direction {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    private enum State {
        IDLE,
        MOVING,
        CHASE,
        ATTACKING
    }

    private Direction direction;
    private State state = State.IDLE;

    // Use this for initialization
    void Start() {
        rigid = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();
        healthBarController = GetComponent<HealthBarController>();
        if(healthBarController == null) {
            Debug.LogError("A health bar is missing");
        }
        healthBarController.SetMaxHealth(health);
        speedChasing = speed * 1.5f;
    }

    // Update is called once per frame
    void Update() {
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        SetDirection();

        //Detection of Player in sight
        if(target == null) {
            CheckPlayerPresence();
        } else {
            destination = target.transform.position;
        }

        switch(state) {
            case State.IDLE:
                currentTimer += Time.deltaTime;
                rigid.velocity = new Vector2(0, 0);

                if(currentTimer >= idleTimer) {
                    currentTimer = 0.0f;

                    //Choose a destination
                    destination = RandomPoint();
                    state = State.MOVING;

                    Vector3 pos1 = ( destination - transform.position ).normalized;
                    Quaternion rotation1 = Quaternion.LookRotation(pos1, Vector3.forward);
                    StartCoroutine(LerpRotation(rotation1));
                }
                break;

            case State.MOVING:
                
                if(Vector3.Distance(transform.position, destination) <= 0.3f) {
                    state = State.IDLE;
                } else {
                    rigid.velocity = (destination - transform.position ).normalized * speed;
                }

                break;

            case State.CHASE:
                rigid.velocity = ( target.transform.position - transform.position ).normalized * speedChasing;

                if(CheckPlayerTouched()) {
                    currentTimer = 0.0f;
                    state = State.ATTACKING;
                }

                Vector3 pos = ( destination - transform.position ).normalized;
                Quaternion rotation = Quaternion.LookRotation(pos);
                sightPoint.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);

                break;

            case State.ATTACKING:
                rigid.velocity = new Vector2(0, 0);

                currentTimer += Time.deltaTime;
                if(currentTimer >= attackTimer) {
                    state = State.CHASE;
                }
                break;
        }

        //Manage animation
        ManageAnimation();
    }

    IEnumerator LerpRotation(Quaternion rotation) {
        float StartTime = Time.time;
        float LerpTime = 1.0f;
        float EndTime = StartTime + LerpTime;
        while(Time.time < EndTime) {
            float timeProgressed = ( Time.time - StartTime ) / LerpTime;  // this will be 0 at the beginning and 1 at the end.

            sightPoint.transform.rotation = Quaternion.Slerp(sightPoint.transform.rotation, rotation, timeProgressed);

            yield return new WaitForFixedUpdate();
        }
    }

    bool CheckPlayerTouched() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + sightPoint.transform.forward / 3, 0.3f, 1 << LayerMask.NameToLayer("Player"));

        foreach(Collider2D collider in colliders) {
            collider.gameObject.SendMessage("TakeDamage", attackPoint, SendMessageOptions.DontRequireReceiver);
            return true;
        }

        return false;
    }

    private void OnDrawGizmos() {
        //Debug affichage zone attaque
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position + sightPoint.transform.forward/3, 0.3f);

        //Debug affichage destination
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(destination, 0.1f);
    }

    Vector3 RandomPoint() {
        bool find = false;
        Vector3 tmpDestination = new Vector3();

        int i = 0;

        while(!find) {
            tmpDestination = (Vector3)Random.insideUnitCircle * 2 + transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, tmpDestination - transform.position, 2, 1 << LayerMask.NameToLayer("Wall"));
            //Debug.DrawRay(transform.position, tmpDestination - transform.position, Color.red, 1.5f);
            if(hit.collider == null) {
                find = true;
            }

            i++;

            if(i > 100) {
                Debug.LogError("FORCE EXIT LOOP");
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
        if(target == null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f, 1 << LayerMask.NameToLayer("Player"));

            foreach(Collider2D collider in colliders) {
                target = collider.gameObject;
                state = State.CHASE;
            }
        }
        health -= damage;

        if(health <= 0) {
            DropValue();
            Destroy(this.gameObject);
        }

        healthBarController.UpdateHealthBar(health);
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
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        if(hitPlayer.collider != null) {

            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Vector2.Distance(transform.position, hitPlayer.transform.position) , 1 << LayerMask.NameToLayer("Wall"));
            if(hitWall.collider == null) {
                Debug.Log("Player vu");
                target = hitPlayer.collider.gameObject;
                state = State.CHASE;
                destination = hitPlayer.transform.position;
            }
        }
    }

    void DropValue() {
        int value = (int)(Random.value * maxValue);
        for(int i = 0; i < value; i++) {
            Vector3 tmpPosition = (Vector3)Random.insideUnitCircle/3 + transform.position;
            Instantiate(coinPrefab, tmpPosition, Quaternion.identity);
        }
    }
}
