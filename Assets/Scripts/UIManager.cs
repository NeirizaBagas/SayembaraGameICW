using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ArusMerah.Data;
using ArusMerah.Managers;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("GamePlay Reference")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timeLimitText;
    [SerializeField] private Slider hookDurabilitySlider;

    [Header("UI Containers")]
    public CanvasGroup mainGameGroup;
    public CanvasGroup shopGroup;
    public CanvasGroup resultGroup;
    public CanvasGroup cutsceneGroup;
    public CanvasGroup endingChoiceGroup;

    [Header("=== Panel Result UI Reference ===")]
    [SerializeField] private TextMeshProUGUI resultGrossEarningsText;
    [SerializeField] private TextMeshProUGUI resultTargetQuotaText;
    [SerializeField] private TextMeshProUGUI resultNetProfitText;
    [SerializeField] private Button nextLevelShopButton;
    [SerializeField] private Button retryLevelButton;

    [Header("Shop UI Reference")]
    [SerializeField] private TextMeshProUGUI shopMoneyText;
    [SerializeField] private TextMeshProUGUI launchSpeedPriceText;
    [SerializeField] private TextMeshProUGUI retrackSpeedPriceText;
    [SerializeField] private TextMeshProUGUI maxDurabilityPriceText;
    [SerializeField] private TextMeshProUGUI repairPriceText;

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f;

    private bool isShopOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        HookMainSystem.OnFishSell += UpdateMoneyUI;
        HookMainSystem.OnHookDurabilityChanged += UpdateDurabilityUI;
        LevelManager.timeUpdate += UpdateTimeLimit;

        UpgradeManager.OnUpgradePurchased += UpdateMoneyUI; // Pastikan UI juga update saat upgrade dibeli
        UpgradeManager.OnLaunchSpeedPriceUpgraded += UpdateLaunchPrice;
        UpgradeManager.OnRetrackSpeedPriceUpgraded += UpdateRetrackPrice;
        UpgradeManager.OnMaxDurabilityPriceUpgrade += UpdateMaxDurabilityPrice;
        UpgradeManager.OnRepairPriceUpgraded += UpdateRepairPrice;

        FlowManager.OnFlowStateChanged += HandleFlowStateChanged;
    }

    private void OnDisable()
    {
        HookMainSystem.OnFishSell -= UpdateMoneyUI;
        HookMainSystem.OnHookDurabilityChanged -= UpdateDurabilityUI;
        LevelManager.timeUpdate -= UpdateTimeLimit;

        UpgradeManager.OnUpgradePurchased -= UpdateMoneyUI;
        UpgradeManager.OnLaunchSpeedPriceUpgraded -= UpdateLaunchPrice;
        UpgradeManager.OnRetrackSpeedPriceUpgraded -= UpdateRetrackPrice;
        UpgradeManager.OnMaxDurabilityPriceUpgrade -= UpdateMaxDurabilityPrice;
        UpgradeManager.OnRepairPriceUpgraded -= UpdateRepairPrice;

        FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
    }

    private void Start()
    {
        // Kondisi awal: Main Game tampil, Shop sembunyi
        SetCanvasState(mainGameGroup, true);
        SetCanvasState(shopGroup, false);
        SetCanvasState(resultGroup, false);
        SetCanvasState(cutsceneGroup, false);
        SetCanvasState(endingChoiceGroup, false);

        UpdateMoneyUI();
    }

    private void HandleFlowStateChanged(GameFlowState newFlowState)
    {
        switch (newFlowState)
        {
            case GameFlowState.GameplayState:
                isShopOpen = false;
                StartCoroutine(TransitionCanvas(currentActiveCanvas(), mainGameGroup));
                UpdateMoneyUI();
                break;
            case GameFlowState.ResultState:
                UpdateResultPanelDisplay();
                if (resultGroup != null)
                {
                    StartCoroutine(TransitionCanvas(currentActiveCanvas(), resultGroup));
                }
                break;
            case GameFlowState.ShopState:
                isShopOpen = true;
                UpdateShopMoneyUI();
                StartCoroutine(TransitionCanvas(currentActiveCanvas(), shopGroup));
                break;
        }
    }

    private CanvasGroup currentActiveCanvas()
    {
        if (shopGroup != null && shopGroup.alpha > 0.5f) return shopGroup;
        if (resultGroup != null && resultGroup.alpha > 0.5f) return resultGroup;
        if (cutsceneGroup != null && cutsceneGroup.alpha > 0.5f) return cutsceneGroup;
        if (endingChoiceGroup != null && endingChoiceGroup.alpha > 0.5f) return endingChoiceGroup;
        return mainGameGroup;
    }

    #region UI MainGame
    public void UpdateDurabilityUI(float ratio) // Update Slider Durability Hook
    {
        if (hookDurabilitySlider != null)
            hookDurabilitySlider.value = ratio;
    }

    public void UpdateTimeLimit(int timeLeft) // Update Text Time Limit
    {
        if (timeLimitText != null)
            timeLimitText.text = timeLeft.ToString();
    }

    public void UpdateMoneyUI() // Update Text Money, dengan target yang diambil langsung dari LevelManager
    {
        // Langsung ambil dari LevelManager untuk targetnya
        int target = (LevelManager.Instance != null) ? LevelManager.Instance.revenueToAchieve : 0 ;
        int grossMoney = (GameData.Instance != null) ? GameData.Instance.grossEarningsToday : 0;

        if (moneyText != null)
        {
            moneyText.text = $"Hasil Melaut: ${grossMoney} / ${target}";
            // Efek Visual: Jika uang kotor sudah mencapai/melebihi target quota, teks berubah hijau
            if (target > 0 && grossMoney >= target)
            {
                moneyText.color = Color.green;
            }
            else
            {
                moneyText.color = Color.white;
            }
        }
    }
    #endregion

    #region Panel Result UI
    public void UpdateResultPanelDisplay()
    {
        int grossMoney = (GameData.Instance != null) ? GameData.Instance.grossEarningsToday : 0;
        int targetQuota = (LevelManager.Instance != null) ? LevelManager.Instance.revenueToAchieve : 0;
        bool isPassed = (grossMoney >= targetQuota);
        if (resultGrossEarningsText != null)
            resultGrossEarningsText.text = grossMoney.ToString();
        if (resultTargetQuotaText != null)
            resultTargetQuotaText.text = targetQuota.ToString();
        if (isPassed)
        {
            int netProfit = grossMoney - targetQuota;
            if (resultNetProfitText != null)
            {
                resultNetProfitText.text = netProfit.ToString();
                resultNetProfitText.color = Color.green;
            }
            // LULUS: Tombol Next Level ke Shop AKTIF, Tombol Retry SEMBUNYI
            if (nextLevelShopButton != null)
            {
                nextLevelShopButton.interactable = true;
            }
            if (retryLevelButton != null)
            {
                retryLevelButton.interactable = false;
            }
        }
        else
        {
            int deficit = targetQuota - grossMoney;
            if (resultNetProfitText != null)
            {
                resultNetProfitText.text = deficit.ToString();
                resultNetProfitText.color = Color.red;
            }
            // GAGAL: Tombol Next Level ke Shop MATI, Tombol Retry AKTIF
            if (nextLevelShopButton != null)
            {
                nextLevelShopButton.interactable = false;
            }
            if (retryLevelButton != null)
            {
                retryLevelButton.interactable = true;
            }
        }
    }

    // Dipanggil saat pemain klik tombol "Lanjut ke Shop" di Panel Result
    public void OnClickNextLevelShopButton()
    {
        if (FlowManager.instance != null)
        {
            FlowManager.instance.ProceedFromResultToShop();
        }
    }
    // Dipanggil saat pemain klik tombol "Ulangi Hari Ini (Retry)" di Panel Result
    public void OnClickRetryButton()
    {
        if (FlowManager.instance != null)
        {
            FlowManager.instance.RetryCurrentLevel();
        }
    }
    #endregion

    #region UI Shop
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

    // Dipanggil saat pemain klik "Hari Berikutnya / Keluar Toko"
    public void OnClickNextDayShopButton()
    {
        if (FlowManager.instance != null)
        {
            FlowManager.instance.ProceedToNextLevelFromShop();
        }
    }
    public void OpenShop()
    {
        StartCoroutine(TransitionCanvas(mainGameGroup, shopGroup));
        isShopOpen = true;
        UpdateShopMoneyUI();
    }
    public void CloseShop()
    {
        StartCoroutine(TransitionCanvas(shopGroup, mainGameGroup));
        isShopOpen = false;
    }
    #endregion

    #region Helper Canvas Crossfade Transition
    private IEnumerator TransitionCanvas(CanvasGroup canvasOut, CanvasGroup canvasIn)
    {
        // 1. Matikan interaksi layar yang lama seketika agar tidak ter-klik dua kali
        canvasOut.interactable = false;
        canvasOut.blocksRaycasts = false;

        float elapsedTime = 0f;

        // 2. Proses Crossfade
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            canvasOut.alpha = 1f - progress; // Fade Out
            canvasIn.alpha = progress;       // Fade In

            yield return null;
        }

        // 3. Pastikan nilai akhir presisi
        canvasOut.alpha = 0f;
        canvasIn.alpha = 1f;

        // 4. Nyalakan interaksi untuk layar yang baru muncul
        canvasIn.interactable = true;
        canvasIn.blocksRaycasts = true;
    }

    private void SetCanvasState(CanvasGroup cg, bool isVisible)
    {
        cg.alpha = isVisible ? 1f : 0f;
        cg.interactable = isVisible;
        cg.blocksRaycasts = isVisible;
    }
    #endregion
}