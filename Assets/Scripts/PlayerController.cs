using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Physics")]
    [SerializeField]
    private float speed;
    [Header("Game logic")]
    [SerializeField]
    private int lifePoint;


    private Rigidbody2D rigid;
    private Animator animatorController;
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
        rigid = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        //Player Mouvement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput == 0) {
            horizontalInput *= rigid.velocity.x * (-1);
        } else {
            horizontalInput *= speed;
        }

        if (verticalInput == 0) {
            verticalInput *= rigid.velocity.y * (-1);
        } else {
            verticalInput *= speed;
        }

        rigid.velocity = new Vector2(horizontalInput, verticalInput);

        //Player Looking at cursor
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var diffX = rigid.position.x - mouse.x;
        var diffY = rigid.position.y - mouse.y;

        if(Mathf.Abs(diffX) > Mathf.Abs(diffY)) {
            if (diffX < 0) {
                direction = Direction.RIGHT;
            } else {
                direction = Direction.LEFT;
            }
        } else  {       
            if (diffY < 0){
                direction = Direction.TOP;
            } else {
                direction = Direction.BOTTOM;
            }
        }

        //Player attacke with sword
        if (Input.GetButtonDown("Fire1") && !isAttackingAnimation) {
            attackWithSword = true;
        }

        //Manage animation
        animatorController.SetBool("lookingTop", false);
        animatorController.SetBool("lookingBottom", false);
        animatorController.SetBool("lookingLeft", false);
        animatorController.SetBool("lookingRight", false);

        animatorController.SetFloat("speed", Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput)));

        switch (direction) {
            case Direction.LEFT:
                animatorController.SetBool("lookingLeft", true);
                if (attackWithSword) {
                    animatorController.SetTrigger("attackSwordLeft");
                    CheckEnemiesTouched(Vector3.left);
                }
                break;

            case Direction.RIGHT:
                animatorController.SetBool("lookingRight", true);
                if (attackWithSword) {
                    animatorController.SetTrigger("attackSwordRight");
                    CheckEnemiesTouched(Vector3.right);
                }
                break;

            case Direction.TOP:
                animatorController.SetBool("lookingTop", true);
                if (attackWithSword) {
                    animatorController.SetTrigger("attackSwordTop");
                    CheckEnemiesTouched(Vector3.up);
                }
                break;

           case Direction.BOTTOM:
                animatorController.SetBool("lookingBottom", true);
                if (attackWithSword) {
                    animatorController.SetTrigger("attackSwordBottom");
                    CheckEnemiesTouched(Vector3.down);
                }
                break;

            default:
                break;
        }

        //Check if can attack
        if (attackWithSword) {
            attackWithSword = false;
            isAttackingAnimation = true;
        }
        
        if (isAttackingAnimation) {
            attackTimer += Time.deltaTime;
            if (attackTimer >= timeBetweenAttack) {
                Debug.Log("Peut attaquer");
                isAttackingAnimation = false;
                attackTimer = 0.0f;
            }
        }
    }

    void CheckEnemiesTouched(Vector2 directionAttack) {
        Debug.DrawLine(transform.position, new Vector2(1,0), Color.white, 5f, false);

        RaycastHit2D hit = Physics2D.Raycast(rigid.position, directionAttack, 1, 1 << LayerMask.NameToLayer("Enemies"));

        if(hit.collider != null) { 
            Debug.Log("There is something in front of the object!");
            hit.collider.gameObject.SendMessage("takeDamage", 5, SendMessageOptions.DontRequireReceiver);
        }
    }
}
