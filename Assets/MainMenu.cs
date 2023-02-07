using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameScene() {
        SceneManager.LoadScene(1);
    }
    public void ExitGame() {
        Application.Quit();
    }
}
