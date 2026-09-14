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
            FlowManager.OnLevelInitiated += UpdateGameplayUI; // Update UI saat level dimulai
            HookMainSystem.OnFishSell += UpdateMoneyUI;
            LevelManager.OnUpdateTarget += HandleTargetUpdated;
            GameData.OnUpdatedGrossEarnings += HandleGrossRevenueUpdated;
            LevelManager.timeUpdate += UpdateTimeLimit;
            HookMainSystem.OnHookDurabilityChanged += UpdateDurabilityUI;
        }
        private void OnDisable()
        {
            FlowManager.OnLevelInitiated -= UpdateGameplayUI; // Unsubscribe dari event saat level dimulai
            HookMainSystem.OnFishSell -= UpdateMoneyUI;
            LevelManager.OnUpdateTarget -= HandleTargetUpdated;
            GameData.OnUpdatedGrossEarnings -= HandleGrossRevenueUpdated;
            LevelManager.timeUpdate -= UpdateTimeLimit;
            HookMainSystem.OnHookDurabilityChanged -= UpdateDurabilityUI;
        }

        private void HandleTargetUpdated(int newTarget)
        {
            currentTargetRevenue = newTarget;
            int walletBalance = (GameData.Instance != null) ? GameData.Instance.walletBalance : 0;
            currentGrossRevenue = walletBalance;
            UpdateMoneyUI();
        }
        private void HandleGrossRevenueUpdated(int grossEarnings)
        {
            int walletBalance = (GameData.Instance != null) ? GameData.Instance.walletBalance : 0;
            currentGrossRevenue = walletBalance + grossEarnings;
            UpdateMoneyUI();
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
            UpdateLevelUI();
            UpdateMoneyUI();
        }

        /// <summary>
        /// Memperbarui teks nomor level pada HUD berdasarkan data level saat ini.
        /// </summary>
        private void UpdateLevelUI()
        {
            if (levelText != null && FlowManager.instance != null && FlowManager.instance.CurrentLevelData != null)
            {
                levelText.text = $"Level: {FlowManager.instance.CurrentLevelData.levelNumber}";
            }
        }

        /// <summary>
        /// Memperbarui teks uang yang tersedia beserta indikator warna pencapaian target pendapatan.
        /// </summary>
        private void UpdateMoneyUI()
        {
            if (moneyText == null) return;

            int totalMoneyAvailable = currentGrossRevenue;

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
