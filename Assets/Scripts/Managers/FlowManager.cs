using ArusMerah.Data;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace ArusMerah.Managers
{
    public enum GameFlowState
    {
        GameplayState,
        ResultState,
        ShopState,
        CutsceneState,
        EndingChoice,
    }

    public class FlowManager : MonoBehaviour
    {
        public static FlowManager instance { get; private set; }

        [Header("Daftar Level Game")]
        [SerializeField] private List<LevelDataSO> listOfAllLevelDataSO;
        private int currentLevelIndex = 0;

        [Header("Status Flow Game Saat Ini")]
        [SerializeField] private GameFlowState currentFlowState;

        // Actions Event
        public static Action<GameFlowState> OnFlowStateChanged;
        public static Action<LevelDataSO> OnLevelDataLoaded;
        public static Action<bool> OnGameplayState;

        public LevelDataSO CurrentLevelData => (currentLevelIndex < listOfAllLevelDataSO.Count) ? listOfAllLevelDataSO[currentLevelIndex] : null; // Getter untuk level data saat ini

        private void Awake()
        {
            if (instance != null & instance != this) Destroy(gameObject);
            else instance = this;
        }

        private void Start()
        {
            currentLevelIndex = 0; // Pastikan level pertama dimulai dari index 0
            LoadLevelByIndex(currentLevelIndex); // Load level pertama saat game dimulai
        }

        public void LoadLevelByIndex(int levelIndex)
        {
            if (listOfAllLevelDataSO == null || listOfAllLevelDataSO.Count == 0) // Cek apakah daftar level kosong
            {
                Debug.LogWarning("[FlowManager]: Daftar LevelDataSO masih kosong!");
                return;
            }
            if (levelIndex < listOfAllLevelDataSO.Count)
            {
                currentLevelIndex = levelIndex;
                LevelDataSO selectedLevelData = listOfAllLevelDataSO[currentLevelIndex];
                // 1. Catat snapshot uang di dompet saat awal hari dimulai (untuk fitur rollback jika kalah)
                if (GameData.Instance != null)
                {
                    GameData.Instance.RecordStartOfDaySnapshot();
                }
                // 2. Broadcast data level aktif
                OnLevelDataLoaded?.Invoke(selectedLevelData);
                // 3. Cek apakah level ini punya Intro Cutscene Koran
                if (selectedLevelData.introCutSceneData != null)
                {
                    ChangeFlowState(GameFlowState.CutsceneState);
                }
                else
                {
                    ChangeFlowState(GameFlowState.GameplayState);
                }
            }
            else
            {
                Debug.Log("[FlowManager]: Selamat! Seluruh level telah selesai dimainkan.");
            }
        }

        public void ChangeFlowState(GameFlowState newFlowState)
        {
            currentFlowState = newFlowState;
            OnFlowStateChanged?.Invoke(newFlowState);
            switch (newFlowState)
            {
                case GameFlowState.CutsceneState:
                    OnGameplayState?.Invoke(false); // Broadcast bahwa gameplay berhenti
                    // UIManager akan memunculkan Canvas Koran
                    break;
                case GameFlowState.GameplayState:
                    StartGameplayState();
                    OnGameplayState?.Invoke(true); // Broadcast bahwa gameplay dimulai
                    break;
                case GameFlowState.ResultState:
                    OnGameplayState?.Invoke(false); // Broadcast bahwa gameplay berhenti
                    // Evaluasi Quota di Panel Result (LevelManager / UIManager)
                    break;
                case GameFlowState.ShopState:
                    OnGameplayState?.Invoke(false); // Broadcast bahwa gameplay berhenti
                    // UIManager akan memunculkan Canvas Shop
                    break;
                //case GameFlowState.InterludeCutscene:
                //    OnGameplayState?.Invoke(false); // Broadcast bahwa gameplay berhenti
                //    // UIManager akan memunculkan Cutscene Selebaran (Lvl 8) / Ajakan Demo (Lvl 9)
                //    break;
                case GameFlowState.EndingChoice:
                    OnGameplayState?.Invoke(false); // Broadcast bahwa gameplay berhenti
                    // UIManager akan memunculkan Layar Pilihan Dual Ending
                    break;
            }
        }

        private void StartGameplayState()
        {
            if (CurrentLevelData == null) return;

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.InitLevelData(CurrentLevelData);
            }

            // 1. Spawn objek di laut via LevelSpawner
            if (LevelSpawner.Instance != null)
            {
                LevelSpawner.Instance.SpawnObjectsForLevel(CurrentLevelData); // Memanggil LevelSpawner untuk spawn objek sesuai data level
            }
            // 2. Inisialisasi timer & target di LevelManager
            
            // 3. Tutup Shop & buka Gameplay HUD di UIManager
            if (UIManager.Instance != null)
            {
                UIManager.Instance.CloseShop();
            }
        }

        // Dipanggil dari UI Panel Result saat Uang Kotor >= Quota Target (Tombol 'Lanjut ke Shop')
        public void ProceedFromResultToShop()
        {
            ChangeFlowState(GameFlowState.ShopState);
        }

        // Dipanggil dari UI Shop saat tombol 'Hari Berikutnya' diklik
        public void ProceedToNextLevelFromShop()
        {
            // Cek apakah ada cutscene interlude post-shop (Level 8 / Level 9)
            if (CurrentLevelData != null && CurrentLevelData.postShopCutSceneData != null)
            {
                ChangeFlowState(GameFlowState.CutsceneState);
            }
            else
            {
                AdvanceToNextLevelIndex();
            }
        }

        public void AdvanceToNextLevelIndex() // Dipanggil dari UI Cutscene saat tombol 'Lanjut' diklik
        {
            currentLevelIndex++;
            LoadLevelByIndex(currentLevelIndex);
        }

        // Dipanggil jika pemain menekan 'Ulangi Hari Ini (Retry)' di Panel Result/Game Over
        public void RetryCurrentLevel()
        {
            // Rollback uang ke snapshot awal hari
            if (GameData.Instance != null)
            {
                GameData.Instance.RollbackMoneyToStartOfDay();
            }
            // Load ulang level yang sama
            LoadLevelByIndex(currentLevelIndex);
        }
    }
}
