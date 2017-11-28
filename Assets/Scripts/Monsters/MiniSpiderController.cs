using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSpiderController : MonsterController {

    enum State {
        CHASE,
        ATTACKING
    }

    State state = State.CHASE;

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody2D>();

        healthBarController = GetComponent<HealthBarController>();
        if(healthBarController == null) {
            Debug.LogError("A health bar is missing");
        }
        healthBarController.SetMaxHealth(health);

        animatorController = GetComponent<Animator>();

        dropController = GetComponent<DropController>();
    }

    // Update is called once per frame
    void Update () {
        switch(state) {
            case State.CHASE:
                rigid.velocity = ( target.transform.position - transform.position ).normalized * speed;

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
        animatorController.SetFloat("speed", Mathf.Max(Mathf.Abs(rigid.velocity.x), Mathf.Abs(rigid.velocity.y)));
        ManageAnimation();
    }

    public override void TakeDamage(int damage) {
        if(target == null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f, 1 << LayerMask.NameToLayer("Player"));

            foreach(Collider2D collider in colliders) {
                target = collider.gameObject.GetComponent<PlayerController>();
            }
        }
        health -= damage;
        if(health <= 0) {
            dropController.DropTreasure();
            Destroy(gameObject);
        }

        healthBarController.UpdateHealthBar(health);

    }

    public void SetTarget(PlayerController recivedTarget) {
        target = recivedTarget;
    }
}
