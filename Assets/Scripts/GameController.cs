using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Pengaturan Progres & Level")]
    public Slider progressSlider;
    public List<GameObject> levels;
    
    [Header("Referensi Objek & UI")]
    public GameObject player;
    public GameObject LoadCanvas;
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public TMP_Text survivedText;

    // Variabel Internal
    private int progressAmount;
    private int currentLevelIndex = 0;
    private int survivedLevelsCount;
    
    // Subscribe/Unsubscribe dari event
    private void OnEnable()
    {
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayedDied += GameOver;
    }

    private void OnDisable()
    {
        Gem.OnGemCollect -= IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete -= LoadNextLevel;
        PlayerHealth.OnPlayedDied -= GameOver;
    }

    void Start()
    {
        progressAmount = 0;
        if (progressSlider != null) progressSlider.value = 0;
        
        if (LoadCanvas != null) LoadCanvas.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (gameWinScreen != null) gameWinScreen.SetActive(false);
    }

    void GameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVELS";
            Time.timeScale = 0;
        }
    }

    void GameWin()
    {
        if (gameWinScreen != null)
        {
            gameWinScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ResetGame()
    {
        // --- PERBAIKAN ERROR ---
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Baris OnReset.Invoke() dihapus dari sini
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        if (progressSlider != null) progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            // --- PERBAIKAN LOGIKA MENANG ---
            if (currentLevelIndex >= levels.Count - 1)
            {
                GameWin();
            }
            else
            {
                if (LoadCanvas != null) LoadCanvas.SetActive(true);
                Debug.Log("Level Complete, hold to load next level!");
            }
        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        if (LoadCanvas != null) LoadCanvas.SetActive(false);

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        if (player != null) player.transform.position = Vector3.zero;

        currentLevelIndex = level;
        progressAmount = 0;
        if (progressSlider != null) progressSlider.value = 0;
        
        if (wantSurvivedIncrease) survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        // Logika di sini tidak perlu diubah, karena sudah ditangani di IncreaseProgressAmount
        int nextLevelIndex = currentLevelIndex + 1;
        if (nextLevelIndex < levels.Count)
        {
            LoadLevel(nextLevelIndex, true);
        }
    }
}