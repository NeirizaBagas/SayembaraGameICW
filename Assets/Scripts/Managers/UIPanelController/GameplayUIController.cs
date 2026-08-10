using ArusMerah.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArusMerah.Managers
{
    public class GameplayUIController : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI timeLimitText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider hookDurabilitySlider;
        private int currentTargetRevenue = 0;
        private int currentGrossRevenue = 0;
        private void OnEnable()
        {
            FlowManager.OnFlowStateChanged += HandleFlowStateChanged;
            FlowManager.OnLevelDataLoaded += HandleLevelName;
            HookMainSystem.OnFishSell += UpdateGameplayUI;
            LevelManager.OnUpdateTarget += HandleTargetUpdated;
            GameData.OnUpdatedGrossEarnings += HandleGrossRevenueUpdated;
            LevelManager.timeUpdate += UpdateTimeLimit;
            HookMainSystem.OnHookDurabilityChanged += UpdateDurabilityUI;
            

            if (FlowManager.instance != null && FlowManager.instance.CurrentFlowState == GameFlowState.GameplayState)
            {
                UpdateGameplayUI();
            }
        }
        private void OnDisable()
        {
            FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
            FlowManager.OnLevelDataLoaded -= HandleLevelName;
            HookMainSystem.OnFishSell -= UpdateGameplayUI;
            LevelManager.OnUpdateTarget -= HandleTargetUpdated;
            GameData.OnUpdatedGrossEarnings -= HandleGrossRevenueUpdated;
            LevelManager.timeUpdate -= UpdateTimeLimit;
            HookMainSystem.OnHookDurabilityChanged -= UpdateDurabilityUI;
        }

        private void HandleFlowStateChanged(GameFlowState newFlowState)
        {
            if (newFlowState == GameFlowState.GameplayState)
            {
                UpdateGameplayUI();
            }
        }

        private void HandleLevelName(LevelDataSO levelData)
        {
            if (levelText != null && levelData != null)
            {
                levelText.text = $"Level: {levelData.levelNumber}";
            }
        }

        private void HandleTargetUpdated(int newTarget)
        {
            currentTargetRevenue = newTarget;
            UpdateGameplayUI();
        }
        private void HandleGrossRevenueUpdated(int grossEarnings)
        {
            currentGrossRevenue = grossEarnings;
            UpdateGameplayUI();
        }
        private void UpdateTimeLimit(int timeLeft)
        {
            if (timeLimitText != null)
                timeLimitText.text = timeLeft.ToString();
        }
        private void UpdateDurabilityUI(float ratio)
        {
            if (hookDurabilitySlider != null)
                hookDurabilitySlider.value = ratio;
        }

        private void UpdateGameplayUI()
        {
            if (moneyText == null) return;

            if (levelText != null && FlowManager.instance != null && FlowManager.instance.CurrentLevelData != null)
            {
                levelText.text = $"Level: {FlowManager.instance.CurrentLevelData.levelNumber}";
            }

            int walletBalance = (GameData.Instance != null) ? GameData.Instance.walletBalance : 0;

            int totalMoneyAvailable = walletBalance + currentGrossRevenue;

            moneyText.text = $"${totalMoneyAvailable} / ${currentTargetRevenue}";
            if (currentTargetRevenue > 0 && totalMoneyAvailable >= currentTargetRevenue)
            {
                moneyText.color = Color.green;
            }
            else
            {
                moneyText.color = Color.white;
            }
        }
    }
}
