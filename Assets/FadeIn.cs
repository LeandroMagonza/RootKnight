using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeIn : MonoBehaviour {
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Calling fade");
        StartCoroutine(FadeInYouWonScreen());
    }

    // Update is called once per frame
    public IEnumerator FadeInYouWonScreen() {
        
        Debug.Log("started fade");
        float alpha = 0;
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        
        for (int i = 0; i < 10; i++) {
            
            yield return new WaitForSeconds(.2f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            alpha += 0.2f;
        }
        StopCoroutine(FadeInYouWonScreen());
    }
}
