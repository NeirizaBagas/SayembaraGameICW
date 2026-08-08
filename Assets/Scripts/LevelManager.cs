using ArusMerah.Data;
using ArusMerah.Managers;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Singleton agar mudah dipanggil

    [Header("Level Settings")]
    public int revenueToAchieve; // Target uang yang harus dikumpulkan untuk menang
    private int targetMoney = 100;
    private int _timeLimit = 60;
    private bool isForcedFailureLevel = false; // Flag khusus Level 10

    private float currentTime;
    private bool isGameActive = true;
    private int lastDisplayedTime = -1; // Untuk mengecek perubahan detik

    private int totalSpawnedItems = 0;
    private int totalCollectedItems = 0;

    // Events untuk UI & Gameplay
    public static Action<int> timeUpdate;
    //public static Action OnLevelComplete;
    public static Action<bool> OnTargetPassed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        LevelSpawner.OnItemSpawned += HandleItemsSpawn;
        HookMainSystem.OnItemClearedFromSea += HandleItemCollected;
        FlowManager.OnFlowStateChanged += HandleFlowStateChanged;
    }

    private void OnDisable()
    {
        LevelSpawner.OnItemSpawned -= HandleItemsSpawn;
        HookMainSystem.OnItemClearedFromSea -= HandleItemCollected;
        FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
    }

    private void Start()
    {

    }

    // Mengambil data langsung dari LevelDataSO yang dikirim oleh FlowManager
    public void InitLevelData(LevelDataSO levelData)
    {
        if (levelData == null) return;
        targetMoney = Mathf.RoundToInt(levelData.targetRevenue);
        revenueToAchieve = targetMoney;
        _timeLimit = levelData.timeLimit;
        isForcedFailureLevel = levelData.isForcedFailureLevel;

        currentTime = _timeLimit;
        lastDisplayedTime = -1;
        totalCollectedItems = 0;
        isGameActive = true;
        Debug.Log($"[LevelManager]: Data Level {levelData.levelNumber} Diterima! Target Quota: ${targetMoney}, Waktu: {_timeLimit}s");
    }

    private void HandleFlowStateChanged(GameFlowState newFlowState)
    {
        if (newFlowState == GameFlowState.GameplayState)
        {
            isGameActive = true;
        }
        else
        {
            isGameActive = false;
        }
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
            EvaluateLevelEnd();
        }
    }

    

    private void HandleItemsSpawn(int totalSpawnedCount)
    {
        totalSpawnedItems = totalSpawnedCount;
        totalCollectedItems = 0; // Reset collected items saat level dimulai
    }



    private void HandleItemCollected()
    {
        if (!isGameActive) return;

        totalCollectedItems++;

        if (totalSpawnedItems > 0 && totalCollectedItems >= totalSpawnedItems)
        {
            EvaluateLevelEnd();
        }
    }

    public void EvaluateLevelEnd()
    {
        if (!isGameActive) return; // Cegah evaluasi ganda
        isGameActive = false;

        int grossEarnings = (GameData.Instance != null) ? GameData.Instance.grossEarningsToday : 0; // Ambil total uang yang dikumpulkan
        bool isQuoataPassed = grossEarnings >= targetMoney;

        if (isForcedFailureLevel)
        {
            //OnLevelComplete();
            OnTargetPassed?.Invoke(false);
            if (FlowManager.instance != null)
            {
                FlowManager.instance.ChangeFlowState(GameFlowState.EndingChoice);
            }
            return;
        }

        if (isQuoataPassed)
        {
            // Potong Quota & Simpan Keuntungan Bersih ke Wallet
            if (GameData.Instance != null)
            {
                GameData.Instance.ApplyQuotaDeductionAndSaveProfit(targetMoney);
            }
            OnTargetPassed?.Invoke(true);
            // Beralih ke Panel Result via FlowManager
            if (FlowManager.instance != null)
            {
                FlowManager.instance.ChangeFlowState(GameFlowState.ResultState);
            }
        }
        else
        {
            // Quota Gagal: Jangan potong, persiapkan opsi Retry
            OnTargetPassed?.Invoke(false);
            if (FlowManager.instance != null)
            {
                FlowManager.instance.ChangeFlowState(GameFlowState.ResultState);
            }
        }
    }
}