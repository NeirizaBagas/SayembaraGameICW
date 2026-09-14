using ArusMerah.Data;
using ArusMerah.Managers;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Singleton agar mudah dipanggil

    [Header("Level Settings")]
    public bool isQuoataPassed = false; // Status apakah quota tercapai atau tidak
    private int targetMoney = 100;
    public int _targetMoney => targetMoney;
    private int _timeLimit = 60;
    private bool isForcedFailureLevel = false; // Flag khusus Level 10

    private float currentTime;
    private bool isGameActive = true;
    private int lastDisplayedTime = -1; // Untuk mengecek perubahan detik
    private int _grossEarnings = 0; // Total uang kotor yang dikumpulkan selama level

    private int totalSpawnedItems = 0;
    private int totalCollectedItems = 0;

    // Events untuk UI & Gameplay
    public static Action<int> timeUpdate;
    public static Action<int> OnUpdateTarget;
    public static Action OnGameCompleted;
    public static Action<bool> OnTargetPassed;
    public static Action<int> OnTargetAchieved;
    public static Action<GameFlowState> OnStateToChange;

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
        HookMainSystem.OnItemClearedFromSea += HandleItemCollected;
        FlowManager.OnFlowStateChanged += HandleFlowStateChanged;
        FlowManager.OnLevelDataLoaded += InitLevelData; // Subscribe ke event untuk menerima data level
        GameData.OnUpdatedGrossEarnings += HandleGrossEarning; // Subscribe ke event untuk menerima data gross earnings
    }

    private void OnDisable()
    {
        HookMainSystem.OnItemClearedFromSea -= HandleItemCollected;
        FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
        FlowManager.OnLevelDataLoaded -= InitLevelData; // Unsubscribe dari event
        GameData.OnUpdatedGrossEarnings -= HandleGrossEarning; // Unsubscribe dari event
    }

    // Mengambil data langsung dari LevelDataSO yang dikirim oleh FlowManager
    public void InitLevelData(LevelDataSO levelData)
    {
        if (levelData == null) return;
        targetMoney = Mathf.RoundToInt(levelData.targetRevenue);
        _timeLimit = levelData.timeLimit;
        isForcedFailureLevel = levelData.isForcedFailureLevel;

        OnUpdateTarget?.Invoke(targetMoney); // Update UI target

        currentTime = _timeLimit;
        lastDisplayedTime = -1;
        totalCollectedItems = 0;
        _grossEarnings = 0;
        isGameActive = true;
    }

    private void HandleFlowStateChanged(GameFlowState newFlowState)
    {
        if (newFlowState == GameFlowState.GameplayState)
        {
            if (currentTime > 0)
            {
                isGameActive = true;
            }
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
    public void HandleItemsSpawn(int totalSpawnedCount)
    {
        totalSpawnedItems = totalSpawnedCount;
        totalCollectedItems = 0; // Reset collected items saat level dimulai
    }

    private void HandleGrossEarning(int amount)
    {
        _grossEarnings = amount;
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
        OnGameCompleted?.Invoke(); // Trigger event untuk memberi tahu bahwa level telah selesai
        isGameActive = false;
        isQuoataPassed = _grossEarnings >= targetMoney;
        Debug.Log(isQuoataPassed);
        if (isForcedFailureLevel)
        {
            OnTargetPassed?.Invoke(false);
            OnStateToChange?.Invoke(GameFlowState.EndingChoiceState);
            return;
        }

        if (isQuoataPassed)
        {
            // Potong Quota & Simpan Keuntungan Bersih ke Wallet
            OnTargetAchieved?.Invoke(targetMoney);
            OnTargetPassed?.Invoke(true);
            // Beralih ke Panel Result via FlowManager
            OnStateToChange?.Invoke(GameFlowState.ResultState);
        }
        else
        {
            // Quota Gagal: Jangan potong, persiapkan opsi Retry
            OnTargetPassed?.Invoke(false);
            OnStateToChange?.Invoke(GameFlowState.ResultState);
        }
    }
}