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

    private int baseMoney = 0;
    private int totalSpawnedItems = 0;
    private int totalCollectedItems = 0;

    public static Action<int> timeUpdate;
    public static Action OnLevelComplete;
    public static Action<string> OnGameOver;

    private void Awake() { Instance = this; }

    private void OnEnable()
    {
        LevelSpawner.OnItemSpawned += HandleItemsSpawn;
        HookMainSystem.OnItemClearedFromSea += HandleItemCollected;
    }

    private void OnDisable()
    {
        LevelSpawner.OnItemSpawned -= HandleItemsSpawn;
        HookMainSystem.OnItemClearedFromSea -= HandleItemCollected;
    }

    private void Start()
    {
        currentTime = timeLimit;
        baseMoney = GameData.Instance.moneyData; // Simpan uang awal saat level dimulai
        // Ambil target money dari GameData jika kamu menyimpannya secara static
        //targetMoney = GameData.CurrentTarget; 
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

    private void HandleItemsSpawn(int totalSpawnedCount)
    {
        totalSpawnedItems = totalSpawnedCount;
        totalCollectedItems = 0; // Reset collected items saat level dimulai
    }

    private void HandleItemCollected()
    {
        totalCollectedItems++;
        if (totalCollectedItems >= totalSpawnedItems)
        {
            CheckWinCondition();
        }
    }

    public void CheckWinCondition()
    {
        isGameActive = false;
        if (GameData.Instance.moneyData >= targetMoney)
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
        OnLevelComplete?.Invoke();
        // Pindah ke scene koran atau tampilkan UI menang
    }

    public void GameOver(string reason)
    {
        isGameActive = false;
        ResetMoney(); // Reset uang ke nilai awal saat level dimulai
        OnGameOver?.Invoke("Gagal membayar biaya hidup harian...");
        Debug.Log("Game Over: " + reason);
        // Tampilkan UI Game Over
    }

    private void ResetMoney() => GameData.Instance.moneyData = baseMoney; // Reset uang ke nilai awal saat level dimulai

    private void UpdateMoney() => GameData.Instance.moneyData = baseMoney; // Update uang ke nilai awal saat level dimulai, bisa dipanggil saat restart level

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Fungsi untuk restart level
}