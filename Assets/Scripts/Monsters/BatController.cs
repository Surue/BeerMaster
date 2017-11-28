using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatController : MonsterController {

    //Variable for target
    [SerializeField]
    private float speedChasing = 5;

    enum State {
        IDLE,
        MOVING,
        CHASE,
        ATTACKING,
        DYING
    }

    State state = State.IDLE;

    BatSoundManager batSoundsManager;

    // Use this for initialization
    void Start() {
        rigid = GetComponent<Rigidbody2D>();

        animatorController = GetComponent<Animator>();

        healthBarController = GetComponent<HealthBarController>();
        if(healthBarController == null) {
            Debug.LogError("A health bar is missing");
        }
        healthBarController.SetMaxHealth(health);

        dropController = GetComponent<DropController>();

        batSoundsManager = GetComponent<BatSoundManager>();

        Collider2D[] tmpGameObject = GetComponents<Collider2D>();
        foreach(Collider2D coll in tmpGameObject) {
            if(!coll.isTrigger) {
                collider2d = coll;
                break;
            }
        }

        lastsPosition = new List<Vector3>();
    }

    // Update is called once per frame
    void Update() {
        SetDirection();

        //Detection of Player in sight
        if(target == null) {
            CheckPlayerPresence();
        } else {
            destination = target.transform.position;
        }

        switch(state) {
            case State.IDLE:
                //Choose a destination
                destination = RandomPoint();
                state = State.MOVING;

                Vector3 pos1 = ( destination - transform.position ).normalized;
                Quaternion rotation1 = Quaternion.LookRotation(pos1, Vector3.forward);
                StartCoroutine(LerpRotation(rotation1));

                if(target != null) {
                    state = State.CHASE;
                }
                break;

            case State.MOVING:
                batSoundsManager.WingSound();
                
                if(IsAtDestination()) {
                    state = State.IDLE;
                } else {
                    rigid.velocity = (destination - transform.position ).normalized * speed;
                }

                if(target != null) {
                    state = State.CHASE;
                }

                if(IsStationnary()) {
                    state = State.IDLE;
                }
                break;

            case State.CHASE:
                batSoundsManager.WingSound();
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

            case State.DYING:
                if(batSoundsManager.FinishDieSound()) {
                    Destroy(gameObject);
                }
                break;
        }

        //Manage animation
        ManageAnimation();
    }

    protected override void PlayDieSound() {
        batSoundsManager.DieSound();
        state = State.DYING;
    }

    private void OnDrawGizmos() {
        //Debug affichage zone attaque
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position + sightPoint.transform.forward/3, 0.3f);

        //Debug affichage destination
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination, 0.1f);
    }

    Vector3 RandomPoint() {
        bool find = false;
        Vector3 tmpDestination = new Vector3();

        while(!find) {
            tmpDestination = (Vector3)Random.insideUnitCircle * 2 + transform.position;

            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, tmpDestination - transform.position, 2, 1 << LayerMask.NameToLayer("Wall"));
            RaycastHit2D hitItem = Physics2D.Raycast(transform.position, tmpDestination - transform.position, 2, 1 << LayerMask.NameToLayer("Item"));
            
            if(hitWall.collider == null && hitItem.collider == null) {
                find = true;
            }
        }

        return tmpDestination;
    }

    protected override void CheckPlayerPresence() {
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        if (hitPlayer.collider != null) {

            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Vector2.Distance(transform.position, hitPlayer.transform.position), 1 << LayerMask.NameToLayer("Wall"));
            if (hitWall.collider == null) {
                target = hitPlayer.collider.gameObject.GetComponent<PlayerController>();
                destination = hitPlayer.transform.position;
            }
        }
    }
}
