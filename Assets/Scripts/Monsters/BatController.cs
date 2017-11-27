﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatController : MonsterController {

    //Variable for target
    private float speedChasing;

    enum State {
        IDLE,
        MOVING,
        CHASE,
        ATTACKING
    }

    State state = State.IDLE;

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

        dropController = GetComponent<DropController>();
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

                if(target != null) {
                    state = State.CHASE;
                }
                break;

            case State.MOVING:
                
                if(Vector3.Distance(transform.position, destination) <= 0.2f) {
                    state = State.IDLE;
                } else {
                    rigid.velocity = (destination - transform.position ).normalized * speed;
                }

                if(target != null) {
                    state = State.CHASE;
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

            RaycastHit2D hit = Physics2D.Raycast(transform.position, tmpDestination - transform.position, 2.5f, 1 << LayerMask.NameToLayer("Wall"));
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
