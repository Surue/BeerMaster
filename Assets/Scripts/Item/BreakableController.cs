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

        dropController = GetComponent<DropController>();
    }

    public void TakeHit() {
        ItemSoundManager.Instance.WoodHitSound();

        hitTaken++;
        if(hitTaken >= sprites.Length) {
            if(dropController != null) {
                dropController.DropTreasure();
            }
            Destroy(gameObject);
        } else {
            spriteRenderer.sprite = sprites[hitTaken];
        }
    }
}
