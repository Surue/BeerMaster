using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonsterController {

    [Header("IA")]
    [SerializeField]
    GameObject nest;
    [SerializeField]
    GameObject prefabMiniSpider;

    SpiderSoundManager spiderSoundsManager;


    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody2D>();

        healthBarController = GetComponent<HealthBarController>();
        if (healthBarController == null) {
            Debug.LogError("A health bar is missing");
        }
        healthBarController.SetMaxHealth(health);

        animatorController = GetComponent<Animator>();

        dropController = GetComponent<DropController>();

        spiderSoundsManager = GetComponent<SpiderSoundManager>();

        Collider2D[] tmpGameObject = GetComponents<Collider2D>();
        foreach(Collider2D coll in tmpGameObject) {
            if(!coll.isTrigger) {
                collider2d = coll;
                break;
            }
        }

        lastsPosition = new List<Vector3>();
    }

    enum State {
        IDLE,
        MOVING,
        CHASE,
        ATTACKING,
        GO_TO_NEST,
        DYING
    }
    State state = State.IDLE;

    // Update is called once per frame
    void Update () {
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        SetDirection();

        //Detection of Player in sight
        if (target == null) {
            CheckPlayerPresence();
        }

        switch (state) {
            case State.IDLE:
                currentTimer += Time.deltaTime;
                rigid.velocity = new Vector2(0, 0);

                if (currentTimer >= idleTimer) {
                    currentTimer = 0.0f;

                    //Choose a destination
                    destination = RandomPoint();
                    state = State.MOVING;

                    Vector3 pos1 = (destination - transform.position).normalized;
                    Quaternion rotation1 = Quaternion.LookRotation(pos1, Vector3.forward);
                    StartCoroutine(LerpRotation(rotation1));
                }

                if(target != null) {
                    destination = nest.transform.position;
                    state = State.GO_TO_NEST;
                }
                break;

            case State.MOVING:
                spiderSoundsManager.ChatteringSound();

                if (IsAtDestination()) {
                    state = State.IDLE;
                }
                else {
                    rigid.velocity = (destination - transform.position).normalized * speed;
                }

                if(target != null) {
                    destination = nest.transform.position;
                    state = State.GO_TO_NEST;
                }

                if(IsStationnary()) {
                    state = State.IDLE;
                }
                break;

            case State.GO_TO_NEST:
                rigid.velocity = ( destination - transform.position ).normalized * speed;
                if(Vector3.Distance(transform.position, destination) <= 0.2f) {
                    Destroy(nest);
                    SpawnMiniSpider();
                    state = State.CHASE;
                }
                break;

            case State.CHASE:
                spiderSoundsManager.ChatteringSound();
                rigid.velocity = (target.transform.position - transform.position).normalized * speed;

                if (CheckPlayerTouched()) {
                    currentTimer = 0.0f;
                    state = State.ATTACKING;
                }

                Vector3 pos = (destination - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(pos);
                sightPoint.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);

                break;

            case State.ATTACKING:
                rigid.velocity = new Vector2(0, 0);

                currentTimer += Time.deltaTime;
                if (currentTimer >= attackTimer) {
                    state = State.CHASE;
                }
                break;

            case State.DYING:
                if(spiderSoundsManager.FinishDieSound()) {
                    Destroy(gameObject);
                }
                break;
        }
        //Manage animation
        animatorController.SetFloat("speed", Mathf.Max(Mathf.Abs(rigid.velocity.x), Mathf.Abs(rigid.velocity.y)));
        ManageAnimation();
    }

    Vector3 RandomPoint() {
        bool find = false;
        Vector3 tmpDestination = new Vector3();

        int i = 0;

        while (!find) {
            tmpDestination = (Vector3)Random.insideUnitCircle * 1.5f + nest.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, tmpDestination - nest.transform.position, 2f, 1 << LayerMask.NameToLayer("Wall"));
            if (hit.collider == null) {
                find = true;
            }

            i++;

            if (i > 100) {
                Debug.LogError("FORCE EXIT LOOP");
                find = true;
            }
        }

        return tmpDestination;
    }

    protected override void CheckPlayerPresence()
    {
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        if (hitPlayer.collider != null)
        {

            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Vector2.Distance(transform.position, hitPlayer.transform.position), 1 << LayerMask.NameToLayer("Wall"));
            if (hitWall.collider == null)
            {
                target = hitPlayer.collider.gameObject.GetComponent<PlayerController>();
            }
        }
    }

    protected override void PlayDieSound() {
        spiderSoundsManager.DieSound();
        state = State.DYING;
    }

    private void OnDrawGizmos() {
        //Debug affichage zone attaque
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position + sightPoint.transform.forward/3, 0.3f);

        //Debug affichage destination
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(destination, 0.1f);
    }

    void SpawnMiniSpider() {
        for(int i = 0; i < 5; i++) {
            Instantiate(prefabMiniSpider, nest.transform.position, Quaternion.identity).GetComponent<MiniSpiderController>().SetTarget(target);
        }
    }

}
