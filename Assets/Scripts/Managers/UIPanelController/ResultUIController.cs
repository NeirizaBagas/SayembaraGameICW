using ArusMerah.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArusMerah.Managers
{
    public class ResultUIController : MonoBehaviour
    {
        [Header("Panel Result UI Reference")]
        [SerializeField] private TextMeshProUGUI resultGrossEarningsText;
        [SerializeField] private TextMeshProUGUI resultTargetQuotaText;
        [SerializeField] private TextMeshProUGUI resultNetProfitText;
        [SerializeField] private Button nextLevelShopButton;
        [SerializeField] private Button retryLevelButton;

        public static Action OnRetryGame;
        public static Action OnOpenShop;

        private void Start()
        {
            // Add Listener di Start HANYA SEKALI (Mencegah bug duplikasi listener)
            if (nextLevelShopButton != null)
                nextLevelShopButton.onClick.AddListener(OnClickNextLevelShopButton);
            if (retryLevelButton != null)
                retryLevelButton.onClick.AddListener(OnClickRetryButton);
        }
        private void OnEnable()
        {
            FlowManager.OnFlowStateChanged += HandleFlowStateChanged;

            if (FlowManager.instance != null && FlowManager.instance.CurrentFlowState == GameFlowState.ResultState)
            {
                UpdateResultPanelDisplay();
            }
        }
        private void OnDisable()
        {
            FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
        }

        private void HandleFlowStateChanged(GameFlowState newFlowState)
        {
            if (newFlowState == GameFlowState.ResultState)
            {
                UpdateResultPanelDisplay();
            }
        }
        public void UpdateResultPanelDisplay()
        {
            int grossMoney = (GameData.Instance != null) ? GameData.Instance.grossEarningsToday : 0;
            int targetQuota = (LevelManager.Instance != null) ? LevelManager.Instance._targetMoney : 0;

            bool isPassed = (grossMoney >= targetQuota);
            Debug.Log(isPassed);


            if (resultGrossEarningsText != null) resultGrossEarningsText.text = $"${grossMoney}";
            if (resultTargetQuotaText != null) resultTargetQuotaText.text = $"${targetQuota}";

            if (isPassed)
            {
                int netProfit = grossMoney - targetQuota;
                if (resultNetProfitText != null)
                {
                    resultNetProfitText.text = $"+${netProfit}";
                    resultNetProfitText.color = Color.green;
                }
                if (nextLevelShopButton != null) nextLevelShopButton.interactable = true;
                if (retryLevelButton != null) retryLevelButton.interactable = false;
            }
            else
            {
                int deficit = targetQuota - grossMoney;
                if (resultNetProfitText != null)
                {
                    resultNetProfitText.text = $"-${deficit}";
                    resultNetProfitText.color = Color.red;
                }
                if (nextLevelShopButton != null) nextLevelShopButton.interactable = false;
                if (retryLevelButton != null) retryLevelButton.interactable = true;
            }
        }
        private void OnClickNextLevelShopButton()
        {
            OnOpenShop?.Invoke();
        }
        private void OnClickRetryButton()
        {
            OnRetryGame?.Invoke();
            Debug.Log("[ResultUIController]: Tombol Retry Dikllick! Membunyikan OnRetryGame...");
        }
    }
}
