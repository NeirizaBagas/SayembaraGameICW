using ArusMerah.Data;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace ArusMerah.Managers
{
    public enum GameFlowState
    {
        MainMenuState,
        GameplayState,
        ResultState,
        ShopState,
        CutsceneState,
        EndingChoiceState,
    }

    public class FlowManager : MonoBehaviour
    {
        public static FlowManager instance { get; private set; }

        [Header("Daftar Level Game")]
        [SerializeField] private List<LevelDataSO> listOfAllLevelDataSO;
        private int currentLevelIndex = 0;

        [Header("Status Flow Game Saat Ini")]
        [SerializeField] private GameFlowState currentFlowState;
        public GameFlowState CurrentFlowState => currentFlowState; // Getter untuk status flow game saat ini

        // Actions Event
        public static Action<GameFlowState> OnFlowStateChanged;
        public static Action<LevelDataSO> OnLevelDataLoaded;
        public static Action OnLevelInitiated;
        public static Action OnRestartGame;

        public LevelDataSO CurrentLevelData => (currentLevelIndex < listOfAllLevelDataSO.Count) ? listOfAllLevelDataSO[currentLevelIndex] : null; // Getter untuk level data saat ini

        private void Awake()
        {
            if (instance != null & instance != this) Destroy(gameObject);
            else instance = this;
        }

        private void OnEnable()
        {
            ResultUIController.OnRetryGame += RetryCurrentLevel;
            ResultUIController.OnOpenShop += ProceedFromResultToShop;
            ShopUIController.OnNextDayShopButtonClicked += ProceedToNextLevelFromShop;
            LevelManager.OnStateToChange += ChangeFlowState;
        }

        private void OnDisable()
        {
            ResultUIController.OnRetryGame -= RetryCurrentLevel;
            ResultUIController.OnOpenShop -= ProceedFromResultToShop;
            ShopUIController.OnNextDayShopButtonClicked -= ProceedToNextLevelFromShop;
            LevelManager.OnStateToChange -= ChangeFlowState;
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
                OnLevelInitiated?.Invoke();
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
                case GameFlowState.MainMenuState:
                    // UIManager akan memunculkan Canvas Main Menu
                    break;
                case GameFlowState.CutsceneState:
                    // UIManager akan memunculkan Canvas Koran
                    break;
                case GameFlowState.GameplayState:
                    StartGameplayState();
                    break;
                case GameFlowState.ResultState:
                    break;
                case GameFlowState.ShopState:
                    ProceedFromResultToShop();
                    break;
                //case GameFlowState.InterludeCutscene:
                //    // UIManager akan memunculkan Cutscene Selebaran (Lvl 8) / Ajakan Demo (Lvl 9)
                //    break;
                case GameFlowState.EndingChoiceState:
                    // UIManager akan memunculkan Layar Pilihan Dual Ending
                    break;
            }
        }

        private void StartGameplayState()
        {
            if (CurrentLevelData == null) return;

            OnLevelDataLoaded?.Invoke(CurrentLevelData);
        }

        // Dipanggil dari UI Panel Result saat Uang Kotor >= Quota Target (Tombol 'Lanjut ke Shop')
        public void ProceedFromResultToShop()
        {
            if (currentFlowState == GameFlowState.ShopState) return; // Jika sudah di Shop, tidak perlu ganti state lagi
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
            Debug.Log("[FlowManager]: Menerima Event Retry! Mengubah State ke GameplayState...");
            // Rollback uang ke snapshot awal hari
            OnRestartGame?.Invoke();
            OnLevelInitiated?.Invoke();
            // 2. MUAT ULANG TIMER & SPAWN IKAN DI LAUT!
            if (CurrentLevelData != null)
            {
                // Panggil event agar LevelManager mereset timer ke 60s
                OnLevelDataLoaded?.Invoke(CurrentLevelData);
                // Spawn ulang ikan & sampah di laut
                if (LevelSpawner.Instance != null)
                {
                    LevelSpawner.Instance.SpawnObjectsForLevel(CurrentLevelData);
                }
            }
            // 3. BARU UBAH STATE KE GAMEPLAY
            ChangeFlowState(GameFlowState.GameplayState);
        }
    }
}
