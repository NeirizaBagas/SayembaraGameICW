using ArusMerah.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArusMerah.Managers
{
    public class ShopUIController : MonoBehaviour
    {
        [Header("Shop UI Reference")]
        [SerializeField] private TextMeshProUGUI shopMoneyText;
        [SerializeField] private TextMeshProUGUI launchSpeedPriceText;
        [SerializeField] private TextMeshProUGUI retrackSpeedPriceText;
        [SerializeField] private TextMeshProUGUI maxDurabilityPriceText;
        [SerializeField] private TextMeshProUGUI repairPriceText;
        [SerializeField] private Button nextDayButton;

        // Actions Event
        public static Action OnNextDayShopButtonClicked;

        private void Start()
        {
            if (nextDayButton != null)
                nextDayButton.onClick.AddListener(OnClickNextDayShopButton);
        }
        private void OnEnable()
        {
            FlowManager.OnFlowStateChanged += HandleFlowStateChanged;
            UpgradeManager.OnUpgradePurchased += UpdateShopMoneyUI;
            UpgradeManager.OnLaunchSpeedPriceUpgraded += UpdateLaunchPrice;
            UpgradeManager.OnRetrackSpeedPriceUpgraded += UpdateRetrackPrice;
            UpgradeManager.OnMaxDurabilityPriceUpgrade += UpdateMaxDurabilityPrice;
            UpgradeManager.OnRepairPriceUpgraded += UpdateRepairPrice;

            if (FlowManager.instance != null && FlowManager.instance.CurrentFlowState == GameFlowState.ShopState)
            {
                UpdateShopMoneyUI();
            }
        }
        private void OnDisable()
        {
            UpgradeManager.OnUpgradePurchased -= UpdateShopMoneyUI;
            UpgradeManager.OnLaunchSpeedPriceUpgraded -= UpdateLaunchPrice;
            UpgradeManager.OnRetrackSpeedPriceUpgraded -= UpdateRetrackPrice;
            UpgradeManager.OnMaxDurabilityPriceUpgrade -= UpdateMaxDurabilityPrice;
            UpgradeManager.OnRepairPriceUpgraded -= UpdateRepairPrice;
            FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
        }
        private void HandleFlowStateChanged(GameFlowState newFlowState)
        {
            if (newFlowState == GameFlowState.ShopState)
            {
                UpdateShopMoneyUI();
            }
        }
        public void UpdateShopMoneyUI()
        {
            if (shopMoneyText != null && GameData.Instance != null)
            {
                shopMoneyText.text = $"Uang Bersih Dompet: ${GameData.Instance.walletBalance}";
            }
        }
        private void UpdateLaunchPrice(int price) => launchSpeedPriceText.text = $"${price}";
        private void UpdateRetrackPrice(int price) => retrackSpeedPriceText.text = $"${price}";
        private void UpdateMaxDurabilityPrice(int price) => maxDurabilityPriceText.text = $"${price}";
        private void UpdateRepairPrice(int price) => repairPriceText.text = $"${price}";
        private void OnClickNextDayShopButton()
        {
            OnNextDayShopButtonClicked?.Invoke();
        }
    }
}
