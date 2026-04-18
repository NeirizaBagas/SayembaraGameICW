using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton agar mudah dipanggil

    [Header("Level Settings")]
    public int targetMoney = 100;
    public int timeLimit = 60;
    private float currentTime;
    private bool isGameActive = true;
    private int lastDisplayedTime = -1; // Untuk mengecek perubahan detik

    private void Awake() { Instance = this; }

    public static Action<int> timeUpdate;

    private void Start()
    {
        currentTime = timeLimit;
        // Ambil target money dari GameData jika kamu menyimpannya secara static
        // targetMoney = GameData.CurrentTarget; 
    }



    private void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;

        // Ubah ke integer untuk mendapatkan angka detik bulat
        int secondsToShow = Mathf.CeilToInt(currentTime);

        // HANYA panggil event jika detiknya berubah (misal dari 60 ke 59)
        if (secondsToShow != lastDisplayedTime)
        {
            lastDisplayedTime = secondsToShow;
            timeUpdate?.Invoke(secondsToShow); // Panggil event
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            CheckWinCondition();
        }
    }

    public void CheckWinCondition()
    {
        isGameActive = false;
        if (GameData.moneyData >= targetMoney)
        {
            WinLevel();
        }
        else
        {
            GameOver("Gagal membayar biaya hidup harian...");
        }
    }

    public void WinLevel()
    {
        Debug.Log("Level Sukses! Masuk ke fase koran/shop.");
        // Pindah ke scene koran atau tampilkan UI menang
    }

    public void GameOver(string reason)
    {
        isGameActive = false;
        Debug.Log("Game Over: " + reason);
        // Tampilkan UI Game Over
    }
}