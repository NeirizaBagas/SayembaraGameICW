using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("GamePlay Reference")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timeLimitText;
    [SerializeField] private Slider hookDurabilitySlider;

    [Header("UI Panels")]
    [SerializeField] private GameObject[] uiElements;
    [SerializeField] private GameObject gameOverContainer;
    [SerializeField] private GameObject levelCompleteContainer;

    [Header("UI Containers")]
    public CanvasGroup mainGameGroup;
    public CanvasGroup shopGroup;
    private bool isShopOpen = false;

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f;

    [Header("Shop UI Reference")]
    [SerializeField] private TextMeshProUGUI shopMoneyText;
    [SerializeField] private TextMeshProUGUI launchSpeedPriceText;
    [SerializeField] private TextMeshProUGUI retrackSpeedPriceText;
    [SerializeField] private TextMeshProUGUI maxDurabilityPriceText;
    [SerializeField] private TextMeshProUGUI repairPriceText;

    private GameObject currentActiveUI;

    public static Action<bool> OnGamePause;

    private void OnEnable()
    {
        HookMainSystem.OnFishSell += UpdateMoneyUI;
        HookMainSystem.OnHookDurabilityChanged += UpdateDurabilityUI;
        LevelManager.timeUpdate += UpdateTimeLimit;
        LevelManager.OnGameOver += OpenGameOverUI;
        LevelManager.OnLevelComplete += OpenLevelCompleteUI;
        UpgradeManager.OnUpgradePurchased += UpdateMoneyUI; // Pastikan UI juga update saat upgrade dibeli
        UpgradeManager.OnLaunchSpeedPriceUpgraded += UpdateLaunchPrice;
        UpgradeManager.OnRetrackSpeedPriceUpgraded += UpdateRetrackPrice;
        UpgradeManager.OnMaxDurabilityPriceUpgrade += UpdateMaxDurabilityPrice;
        UpgradeManager.OnRepairPriceUpgraded += UpdateRepairPrice;
    }

    private void OnDisable()
    {
        HookMainSystem.OnFishSell -= UpdateMoneyUI;
        HookMainSystem.OnHookDurabilityChanged -= UpdateDurabilityUI;
        LevelManager.timeUpdate -= UpdateTimeLimit;
        LevelManager.OnGameOver -= OpenGameOverUI;
        LevelManager.OnLevelComplete -= OpenLevelCompleteUI;
        UpgradeManager.OnUpgradePurchased -= UpdateMoneyUI;
        UpgradeManager.OnLaunchSpeedPriceUpgraded -= UpdateLaunchPrice;
        UpgradeManager.OnRetrackSpeedPriceUpgraded -= UpdateRetrackPrice;
        UpgradeManager.OnMaxDurabilityPriceUpgrade -= UpdateMaxDurabilityPrice;
        UpgradeManager.OnRepairPriceUpgraded -= UpdateRepairPrice;
    }

    private void Start()
    {
        // Kondisi awal: Main Game tampil, Shop sembunyi
        SetCanvasState(mainGameGroup, true);
        SetCanvasState(shopGroup, false);

        UpdateMoneyUI();
        CloseAllUI();
    }
    #region UI MainGame
    public void UpdateDurabilityUI(float ratio) // Update Slider Durability Hook
    {
        hookDurabilitySlider.value = ratio;
    }

    public void UpdateTimeLimit(int timeLeft) // Update Text Time Limit
    {
        timeLimitText.text = timeLeft.ToString();
    }

    public void UpdateMoneyUI() // Update Text Money, dengan target yang diambil langsung dari LevelManager
    {
        // Langsung ambil dari LevelManager untuk targetnya
        int target = LevelManager.Instance.targetMoney;
        moneyText.text = $"Money: {GameData.Instance.moneyData} / {target}";

        // Efek visual: Kalau target tercapai, ganti warna teks jadi hijau
        if (GameData.Instance.moneyData >= target)
        {
            moneyText.color = Color.green;
        }

        if (isShopOpen)
        {
            shopMoneyText.text = $"Money: {GameData.Instance.moneyData}";
        }
    }

    public void OpenUi(GameObject uiToOpen) // Fungsi umum untuk membuka UI, akan menutup UI lain yang sedang aktif
    {
        if (currentActiveUI == uiToOpen && uiToOpen.activeSelf) return;

        if (currentActiveUI != null) currentActiveUI.SetActive(false);

        if (uiToOpen != null)
        {
            uiToOpen.SetActive(true);
            currentActiveUI = uiToOpen;
            Debug.Log("Opening UI: " + uiToOpen.name);
        }
    }

    public void CloseCurrentUI() // Fungsi untuk menutup UI yang sedang aktif
    {
        if (currentActiveUI != null)
        {
            currentActiveUI.SetActive(false);
            currentActiveUI = null;
            Debug.Log("Closing Current UI");
        }
    }

    private void CloseAllUI() // Fungsi untuk menutup semua UI, bisa dipanggil saat memulai level atau saat kondisi tertentu
    {
        foreach (GameObject ui in uiElements)
        {
            if (ui != null && ui.activeSelf)
            {
                ui.SetActive(false);
            }
        }
        currentActiveUI = null;
    }

    private void OpenGameOverUI(string reason) // Fungsi untuk membuka UI Game Over, dengan alasan yang ditampilkan
    {
        OnGamePause?.Invoke(true); 
        OpenUi(gameOverContainer);
        TextMeshProUGUI reasonText = gameOverContainer.GetComponentInChildren<TextMeshProUGUI>();
        if (reasonText != null)
        {
            reasonText.text = "Game Over: " + reason;
        }
    }

    private void OpenLevelCompleteUI()
    {
        OnGamePause?.Invoke(true);
        OpenUi(levelCompleteContainer); // Fungsi untuk membuka UI Level Complete
    }
    #endregion

    #region UI Shop
    // Panggil fungsi ini di onClick Button "Buka Shop"
    public void OpenShop()
    {
        StartCoroutine(TransitionCanvas(mainGameGroup, shopGroup));
        isShopOpen = true;
        UpdateMoneyUI(); // Pastikan text money di shop selalu update saat dibuka
    }

    private void UpdatePriceUpgradeItem()
    {

    }

    private void UpdateLaunchPrice(int price) => launchSpeedPriceText.text = price.ToString();

    private void UpdateRetrackPrice(int price) => retrackSpeedPriceText.text = price.ToString();

    private void UpdateMaxDurabilityPrice(int price) => maxDurabilityPriceText.text = price.ToString();

    private void UpdateRepairPrice(int price) => repairPriceText.text = price.ToString();

    // Panggil fungsi ini di onClick Button "Kembali" atau "Tutup Shop"
    public void CloseShop()
    {
        StartCoroutine(TransitionCanvas(shopGroup, mainGameGroup));
    }

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