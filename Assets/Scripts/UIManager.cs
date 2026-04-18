using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timeLimitText;
    [SerializeField] private Slider hookDurabilitySlider;

    private void OnEnable()
    {
        HookMainSystem.OnFishSell += UpdateUI;
        HookMainSystem.OnHookDurabilityChanged += UpdateDurabilityUI;
        LevelManager.timeUpdate += UpdateTimeLimit;
    }

    private void OnDisable()
    {
        HookMainSystem.OnFishSell -= UpdateUI;
        HookMainSystem.OnHookDurabilityChanged -= UpdateDurabilityUI;
        LevelManager.timeUpdate -= UpdateTimeLimit;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateDurabilityUI(float ratio)
    {
        hookDurabilitySlider.value = ratio;
    }

    public void UpdateTimeLimit(int timeLeft)
    {
        timeLimitText.text = timeLeft.ToString();
    }

    public void UpdateUI()
    {
        // Langsung ambil dari LevelManager untuk targetnya
        int target = LevelManager.Instance.targetMoney;
        moneyText.text = $"Money: {GameData.moneyData} / {target}";

        // Efek visual: Kalau target tercapai, ganti warna teks jadi hijau
        if (GameData.moneyData >= target)
        {
            moneyText.color = Color.green;
        }
    }
}