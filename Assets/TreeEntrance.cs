using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Platformer.Mechanics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TreeEntrance : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxHp = 500;
    public float hp = 500;
    public float timer = 120;
    public TextMeshProUGUI timerDisplay;
    public Slider treeHpIndicator;

    public AudioClip in_game_music;
    public AudioClip in_game_music_last_seconds;
    public AudioClip defeat_music;
    public AudioClip winMusic;
    
    public bool lastSeconds = false;
    
    public AudioSource music;

    public PlayerController player;

    public float respawnTimer = 5f;
    public float currentRespawnTimer = 0f;

    public GameObject gameOverScreen;
    public List<TextMeshProUGUI> gameOverTexts;
    public Image gameOverPanel;
   
    public GameObject youWonScreen;
    public List<TextMeshProUGUI> youWonTexts;
    public Image youWonPanel;
    
    [SerializeField] public bool gameOver = false;

    void Start()
    {
        treeHpIndicator.maxValue = maxHp;
        treeHpIndicator.value = hp;
        music.clip = in_game_music;
        music.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameOver) return;
        
        if (timer > 0) {
            timer -= Time.deltaTime;
            if (timer < 0) {
                Debug.Log("Ganaste");
                Win();
                return;
            }
        }
        float minutes = Mathf.FloorToInt(timer / 60);  
        float seconds = Mathf.FloorToInt(timer % 60);
        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timer<30f && !lastSeconds) {
            lastSeconds = true;
            music.clip = in_game_music_last_seconds;
            music.Play();
        }

        if (currentRespawnTimer > 0) {
            currentRespawnTimer -= Time.deltaTime;
            if (currentRespawnTimer < 0) {
                RespawnPlayer();
            }
        }
    }

    private void RespawnPlayer() {
        ReceiveDamage(player.maxHp);
        player.Respawn();
        player.gameObject.SetActive(true);
    }

    public void OnPlayerDeath() {
        currentRespawnTimer = respawnTimer;
        player.gameObject.SetActive(false);

    }

    
    public void ReceiveDamage(float treeDamage) {
        if (gameOver) return;

        hp -= treeDamage;
        treeHpIndicator.value = hp;
        
        if (hp > maxHp) {
            hp = maxHp;
        }
        
        if (hp <= 0) {
            hp = 0;
            Die();
        }
        

    }

    private void Die() {
        gameOver = true;
        Debug.Log("Startign fade");
        music.clip = defeat_music;
        music.Play();
        StartCoroutine(FadeInGameOverScreen());
    }    
    private void Win() {
        gameOver = true;
        Debug.Log("Startign fade");
        music.clip = winMusic;
        music.Play();
        StartCoroutine(FadeInYouWonScreen());
    }

    public IEnumerator FadeInGameOverScreen() {
        
        foreach (var text in gameOverTexts) {
          text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        gameOverPanel.color = new Color(gameOverPanel.color.r, gameOverPanel.color.g, gameOverPanel.color.b, 0);
        
        gameOverScreen.SetActive(true);
        float alpha = 0;
        for (int i = 0; i < 10; i++) {
            
            yield return new WaitForSeconds(.2f);
            foreach (var text in gameOverTexts) {
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            }

            gameOverPanel.color = new Color(gameOverPanel.color.r, gameOverPanel.color.g, gameOverPanel.color.b, alpha);
            alpha += 0.2f;
        }
    }
    public IEnumerator FadeInYouWonScreen() {
        
        foreach (var text in youWonTexts) {
          text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        youWonPanel.color = new Color(youWonPanel.color.r, youWonPanel.color.g, youWonPanel.color.b, 0);
        
        youWonScreen.SetActive(true);
        float alpha = 0;
        for (int i = 0; i < 10; i++) {
            
            yield return new WaitForSeconds(.2f);
            foreach (var text in youWonTexts) {
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            }

            youWonPanel.color = new Color(youWonPanel.color.r, youWonPanel.color.g, youWonPanel.color.b, alpha);
            alpha += 0.2f;
        }
    }

    public void ReloadScene() {
        SceneManager.LoadScene(1);
    }
    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
