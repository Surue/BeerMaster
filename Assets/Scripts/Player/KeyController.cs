using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyController : MonoBehaviour {
    
    [SerializeField]
    Image keyImagePrefab;
    [SerializeField]
    GameObject positionToPlaceKeySprite;
    [SerializeField]
    GameObject canvas;


    private int keyInInventory = 0;

    const int offsetYForKeyImage = 32;

    List<Image> spritsDisplayed;

    // Use this for initialization
    void Start () {
        spritsDisplayed = new List<Image>();
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void AddKey()
    {
        keyInInventory++;
        Image tmpKeyImage = Instantiate(
            keyImagePrefab,
            positionToPlaceKeySprite.transform.position + new Vector3(0, offsetYForKeyImage * keyInInventory,0),
            Quaternion.identity);

        tmpKeyImage.transform.SetParent(canvas.transform, true);

        spritsDisplayed.Add(tmpKeyImage);
    }

    public void UseKey()
    {
        keyInInventory--;
        Destroy(spritsDisplayed[spritsDisplayed.Count - 1]);
        spritsDisplayed.RemoveAt(spritsDisplayed.Count - 1);
    }

    public bool HasKey()
    {
        return keyInInventory > 0;
    }
}
