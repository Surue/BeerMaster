using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {

    [SerializeField]
    private int valueTreasure;
    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private GameObject placeToDrop;

    private const float radiusDivisor = 3;

    // Use this for initialization
    void Start () {
		if(placeToDrop == null) {
            placeToDrop = gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DropTreasure() {
        ItemSoundManager.Instance.CoinSound();
        if(placeToDrop == null) {
            placeToDrop = this.gameObject;
        }
        for(int i = 0; i < valueTreasure; i++) {
            Vector3 tmpPosition = (Vector3)Random.insideUnitCircle / radiusDivisor + placeToDrop.transform.position;
            Instantiate(coinPrefab, tmpPosition, Quaternion.identity);
        }
    }
}
