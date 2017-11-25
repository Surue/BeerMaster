using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    [Header("Physics")]
    [SerializeField]
    protected float speed = 1;
    [Header("Game logic")]
    [SerializeField]
    protected int health = 20;
    [SerializeField]
    protected GameObject sightPoint;
    [SerializeField]
    private int attackPoint = 1;
    [Header("Loot")]
    [SerializeField]
    private int treasureValue;
    [SerializeField]
    private GameObject coinPrefab;

    private int maxHealth;
    protected HealthBarController healthBarController;
    protected Rigidbody2D rigid;
    protected Animator animatorController;
    private DropController dropController;
    protected Vector3 destination;

    //Variable for target
    protected PlayerController target;

    //Timer for IA
    protected float currentTimer = 0.0f;
    protected float idleTimer = 2.0f;
    protected float attackTimer = .5f;

    protected enum Direction {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    protected enum State {
        IDLE,
        MOVING,
        CHASE,
        ATTACKING
    }

    protected Direction direction;
    protected State state = State.IDLE;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    protected IEnumerator LerpRotation(Quaternion rotation) {
        float StartTime = Time.time;
        float LerpTime = 1.0f;
        float EndTime = StartTime + LerpTime;
        while (Time.time < EndTime) {
            float timeProgressed = (Time.time - StartTime) / LerpTime;  // this will be 0 at the beginning and 1 at the end.

            sightPoint.transform.rotation = Quaternion.Slerp(sightPoint.transform.rotation, rotation, timeProgressed);

            yield return new WaitForFixedUpdate();
        }
    }

    protected void SetDirection() {
        if (Mathf.Abs(sightPoint.transform.forward.x) > Mathf.Abs(sightPoint.transform.forward.y)) {
            if (sightPoint.transform.forward.x > 0) {
                direction = Direction.RIGHT;
            }
            else {
                direction = Direction.LEFT;
            }
        }
        else {
            if (sightPoint.transform.forward.y > 0) {
                direction = Direction.UP;
            }
            else {
                direction = Direction.DOWN;
            }
        }
    }

    public void TakeDamage(int damage) {
        if (target == null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f, 1 << LayerMask.NameToLayer("Player"));

            foreach (Collider2D collider in colliders) {
                target = collider.gameObject.GetComponent<PlayerController>();
                state = State.CHASE;
            }
        }
        health -= damage;
        if (health <= 0) {
            dropController.DropTreasure();
            Destroy(this.gameObject);
        }

        healthBarController.UpdateHealthBar(health);
    }

    protected virtual void CheckPlayerPresence() {
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        if (hitPlayer.collider != null) {

            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Vector2.Distance(transform.position, hitPlayer.transform.position), 1 << LayerMask.NameToLayer("Wall"));
            if (hitWall.collider == null) {
                Debug.Log("Player vu");
                target = hitPlayer.collider.gameObject.GetComponent<PlayerController>();
                state = State.CHASE;
                destination = hitPlayer.transform.position;
            }
        }
    }

    //When monster moving, test if the player is near, if it's the case then the player take damage
    protected bool CheckPlayerTouched() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + sightPoint.transform.forward / 3, 0.3f, 1 << LayerMask.NameToLayer("Player"));

        foreach (Collider2D collider in colliders) {
            target.TakeDamage(attackPoint);
            return true;
        }

        return false;
    }

    protected void ManageAnimation() {

        animatorController.SetBool("isLookingUp", false);
        animatorController.SetBool("isLookingDown", false);
        animatorController.SetBool("isLookingLeft", false);
        animatorController.SetBool("isLookingRight", false);

        switch (direction) {
            case Direction.LEFT:
                animatorController.SetBool("isLookingLeft", true);
                break;

            case Direction.RIGHT:
                animatorController.SetBool("isLookingRight", true);
                break;

            case Direction.UP:
                animatorController.SetBool("isLookingUp", true);
                break;

            case Direction.DOWN:
                animatorController.SetBool("isLookingDown", true);
                break;

            default:
                break;
        }
    }
}
