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
        public GameFlowState CurrentFlowState => currentFlowState;

        public static Action<List<LevelDataSO>> OnInitializePoolLevels;
        public static Action<GameFlowState> OnFlowStateChanged;
        public static Action<LevelDataSO> OnLevelDataLoaded;
        public static Action OnLevelInitiated;
        public static Action OnRestartGame;

        public LevelDataSO CurrentLevelData => (currentLevelIndex < listOfAllLevelDataSO.Count) ? listOfAllLevelDataSO[currentLevelIndex] : null;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
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
            OnInitializePoolLevels?.Invoke(listOfAllLevelDataSO);
            currentLevelIndex = 0;
            LoadLevelByIndex(currentLevelIndex);
        }

        public void LoadLevelByIndex(int levelIndex)
        {
            if (listOfAllLevelDataSO == null || listOfAllLevelDataSO.Count == 0)
            {
                Debug.LogWarning("[FlowManager]: Daftar LevelDataSO masih kosong!");
                return;
            }

            if (levelIndex < listOfAllLevelDataSO.Count)
            {
                currentLevelIndex = levelIndex;
                LevelDataSO selectedLevelData = listOfAllLevelDataSO[currentLevelIndex];
                OnLevelInitiated?.Invoke();

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
                    break;
                case GameFlowState.CutsceneState:
                    break;
                case GameFlowState.GameplayState:
                    StartGameplayState();
                    break;
                case GameFlowState.ResultState:
                    break;
                case GameFlowState.ShopState:
                    ProceedFromResultToShop();
                    break;
                case GameFlowState.EndingChoiceState:
                    break;
            }
        }

        private void StartGameplayState()
        {
            if (CurrentLevelData == null) return;
            OnLevelDataLoaded?.Invoke(CurrentLevelData);
        }

        public void ProceedFromResultToShop()
        {
            if (currentFlowState == GameFlowState.ShopState) return;
            ChangeFlowState(GameFlowState.ShopState);
        }

        public void ProceedToNextLevelFromShop()
        {
            if (CurrentLevelData != null && CurrentLevelData.postShopCutSceneData != null)
            {
                ChangeFlowState(GameFlowState.CutsceneState);
            }
            else
            {
                AdvanceToNextLevelIndex();
            }
        }

        public void AdvanceToNextLevelIndex()
        {
            currentLevelIndex++;
            LoadLevelByIndex(currentLevelIndex);
        }

        public void RetryCurrentLevel()
        {
            Debug.Log("[FlowManager]: Menerima Event Retry! Mengubah State ke GameplayState...");
            OnRestartGame?.Invoke();
            OnLevelInitiated?.Invoke();

            if (CurrentLevelData != null)
            {
                OnLevelDataLoaded?.Invoke(CurrentLevelData);
            }

            ChangeFlowState(GameFlowState.GameplayState);
        }
    }
}
