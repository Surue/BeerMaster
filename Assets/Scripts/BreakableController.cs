using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableController : MonoBehaviour {

    [SerializeField]
    Sprite[] sprites;

    int hitTaken = 0;
    SpriteRenderer spriteRenderer;

    DropController dropController;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[hitTaken];

        dropController = GetComponent<DropController>();
    }

    public void TakeHit() {
        hitTaken++;
        if(hitTaken >= sprites.Length) {
            dropController.DropTreasure();
            Destroy(gameObject);
        } else {
            spriteRenderer.sprite = sprites[hitTaken];
        }
    }
}
