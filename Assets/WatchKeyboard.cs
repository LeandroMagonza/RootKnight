using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchKeyboard : MonoBehaviour {
    public Sprite unpressed;
    public Sprite pressed;
    public string observed;
    
    public Image imageToShow;

    public bool isKey;
    // Start is called before the first frame update
    void Start() {
        imageToShow = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((!isKey && Input.GetButton(observed)) || (isKey && Input.GetKey(observed))) {
            imageToShow.sprite = pressed;
        }
        else { 
            imageToShow.sprite = unpressed;
        }
        
    }
}
