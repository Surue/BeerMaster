﻿using System.Collections;
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
    
    protected HealthBarController healthBarController;
    protected Rigidbody2D rigid;
    protected Animator animatorController;
    protected DropController dropController;
    protected Vector3 destination;
    protected Collider2D collider2d;

    protected List<Vector3> lastsPosition;
    private const int maxLastPosition = 5;

    //Variable for target
    protected PlayerController target;

    //Timer for IA
    protected float currentTimer = 0.0f;
    protected float idleTimer = 2.0f;
    protected float attackTimer = .5f;

    private const float radiusDivisor = 3;

    protected enum Direction {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    protected Direction direction;

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

    protected virtual void PlayDieSound() {
    }

    protected bool IsAtDestination() {
        return collider2d.bounds.Contains(destination);
    }

    public virtual void TakeDamage(int damage) {
        if (target == null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f, 1 << LayerMask.NameToLayer("Player"));

            foreach (Collider2D collider in colliders) {
                target = collider.gameObject.GetComponent<PlayerController>();
            }
        }
        health -= damage;
        if (health <= 0) {
            dropController.DropTreasure();
            PlayDieSound();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            healthBarController.HideHealthBar();
        }

        healthBarController.UpdateHealthBar(health);

    }

    protected virtual void CheckPlayerPresence() {
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, sightPoint.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));
        Debug.DrawRay(transform.position, sightPoint.transform.forward);
        if(hitPlayer.collider != null) {

            RaycastHit2D hitWall = Physics2D.Raycast(transform.position, 
                                                    sightPoint.transform.forward, 
                                                    Vector2.Distance(transform.position, hitPlayer.transform.position), 
                                                    1 << LayerMask.NameToLayer("Wall"));
            if(hitWall.collider == null) {
                target = hitPlayer.collider.gameObject.GetComponent<PlayerController>();
            }
        }
    }

    //Check if a certain number are all the same, if it's the case then it meen the monster is stationnary
    protected bool IsStationnary() {
        lastsPosition.Add(transform.position);
        if(lastsPosition.Count > maxLastPosition) {
            lastsPosition.RemoveAt(0);

            Vector3 firstPos = new Vector3();

            foreach(Vector3 pos in lastsPosition) {
                if(firstPos == new Vector3()) {
                    firstPos = pos;
                } else {
                    if(pos != firstPos) {
                        return false;
                    }
                }
            }
            return true;
        }

        return false;
    }

    //When monster moving, test if the player is near, if it's the case then the player take damage
    protected bool CheckPlayerTouched() {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + sightPoint.transform.forward / radiusDivisor, 0.3f, 1 << LayerMask.NameToLayer("Player"));

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
