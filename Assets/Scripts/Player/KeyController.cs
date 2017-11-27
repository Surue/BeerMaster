using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyController : MonoBehaviour {

    [SerializeField]
    Sprite sourceImage;
    [SerializeField]
    Image keyImagePrefab;
    [SerializeField]
    GameObject positionToPlaceKeySprite;


    private int keyInInventory = 0;

    List<Image> spritsDisplayed;

    // Use this for initialization
    void Start () {
        Image test = Instantiate(
            keyImagePrefab,
            positionToPlaceKeySprite.transform.position,
            Quaternion.identity);
        test.sprite = sourceImage;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddKey()
    {
        keyInInventory++;
        spritsDisplayed.Add(Instantiate(
            keyImagePrefab, 
            new Vector3(positionToPlaceKeySprite.transform.position.x + keyInInventory* keyImagePrefab.GetComponent<RectTransform>().rect.width,
                        positionToPlaceKeySprite.transform.position.y,
                        positionToPlaceKeySprite.transform.position.z), 
            Quaternion.identity));
    }

    public void UseKey()
    {
        keyInInventory--;
        spritsDisplayed[keyInInventory + 1] = null;
    }

    public bool HasKey()
    {
        return keyInInventory > 0;
    }
}
