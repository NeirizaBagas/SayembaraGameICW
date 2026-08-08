using System;
using TMPro;
using UnityEngine;
using ArusMerah.Data;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warningText; // Tulisan "Uang Tidak Cukup"

    [Header("Upgrade Prices")]
    [SerializeField] private int launchSpeedUpgradePrice = 100;
    [SerializeField] private int retrackSpeedUpgradePrice = 80;
    [SerializeField] private int maxDurabilityUpgradePrice = 150;
    [SerializeField] private int repairPrice = 50;

    [Header("Pengali Kenaikan Harga Setelah Dibeli")]
    [SerializeField] private int multiplierPrice; // Variabel umum untuk menyimpan harga upgrade yang sedang dibeli

    [Header("Nilai Penambahan Stats")]
    [SerializeField] private float launchSpeedAddValue = 1.5f;
    [SerializeField] private float retrackSpeedAddValue = 1.0f;
    [SerializeField] private float maxDurabilityAddValue = 25f;

    private bool isUpgradingMaxDurability = false; // Flag untuk mengecek apakah upgrade max durability sedang dibeli

    public static Action OnUpgradePurchased; // Event untuk memberi tahu UI agar update setelah upgrade dibeli
    public static Action<int> OnLaunchSpeedPriceUpgraded; // Event khusus untuk upgrade launch speed
    public static Action<int> OnRetrackSpeedPriceUpgraded; // Event khusus untuk upgrade retrack speed
    public static Action<int> OnMaxDurabilityPriceUpgrade;
    public static Action<int> OnRepairPriceUpgraded;

    private void Start()
    {
        BroadcastAllPrices();
    }

    public void BroadcastAllPrices()
    {
        OnLaunchSpeedPriceUpgraded?.Invoke(launchSpeedUpgradePrice);
        OnRetrackSpeedPriceUpgraded?.Invoke(retrackSpeedUpgradePrice);
        OnMaxDurabilityPriceUpgrade?.Invoke(maxDurabilityUpgradePrice);
        OnRepairPriceUpgraded?.Invoke(repairPrice);
    }

    // Fungsi ini dipanggil oleh Button Upgrade Speed di Inspector
    public void BuyUpgradeLaunchSpeed()
    {
        if(GameData.Instance != null && GameData.Instance.TrySpendMoney(launchSpeedUpgradePrice)) // Cek apakah uang cukup dan kurangi uang dari dompet
        {
            GameData.Instance.UpdateLaunchSpeed(launchSpeedAddValue); // Panggil fungsi untuk menambah kecepatan launch
            launchSpeedUpgradePrice = Mathf.RoundToInt(launchSpeedUpgradePrice * multiplierPrice); // Naikkan harga untuk upgrade berikutnya

            OnLaunchSpeedPriceUpgraded?.Invoke(launchSpeedUpgradePrice); // Panggil event untuk update UI
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
            Debug.Log("[UpgradeManager]: Upgrade Launch Speed Berhasil!");
        }
        else
        {
            ShowWarning("Uang Dompet Tidak Cukup!");
        }
    }

    // Dipanggil oleh Button Upgrade Retrack Speed di UI Shop
    public void BuyUpgradeRetrackSpeed()
    {
        if (GameData.Instance != null && GameData.Instance.TrySpendMoney(retrackSpeedUpgradePrice))
        {
            GameData.Instance.UpdateRetrackSpeed(retrackSpeedAddValue); // Panggil fungsi untuk menambah kecepatan retrack
            retrackSpeedUpgradePrice = Mathf.RoundToInt(retrackSpeedUpgradePrice * multiplierPrice); // Naikkan harga untuk upgrade berikutnya
            OnRetrackSpeedPriceUpgraded?.Invoke(retrackSpeedUpgradePrice); // Panggil event untuk update UI
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
            Debug.Log("[UpgradeManager]: Upgrade Retract Speed Berhasil!");
        }
        else
        {
            ShowWarning("Uang Dompet Tidak Cukup!");
        }
    
    }

    // Dipanggil oleh Tombol Upgrade Max Durability di UI Shop
    public void BuyUpgradeMaxDurability()
    {
        if (GameData.Instance != null && GameData.Instance.TrySpendMoney(maxDurabilityUpgradePrice))
        {
            GameData.Instance.UpdateMaxHookDurability(maxDurabilityAddValue); // Panggil fungsi untuk menambah max durability
            maxDurabilityUpgradePrice = Mathf.RoundToInt(maxDurabilityUpgradePrice * multiplierPrice); // Naikkan harga untuk upgrade berikutnya

            isUpgradingMaxDurability = true; // Set flag untuk menandai bahwa upgrade max durability sedang dibeli
            OnMaxDurabilityPriceUpgrade?.Invoke(maxDurabilityUpgradePrice); // Panggil event untuk update UI
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
            Debug.Log("[UpgradeManager]: Upgrade Max Durability Berhasil!");
        }
        else
        {
            ShowWarning("Uang Dompet Tidak Cukup!");
        }
    }

    // Dipanggil oleh Tombol Perbaiki Kail di UI Shop
    public void BuyRepair()
    {
        if (isUpgradingMaxDurability)
        {
            repairPrice = Mathf.RoundToInt(repairPrice * multiplierPrice);
            isUpgradingMaxDurability = false;
            OnRepairPriceUpgraded?.Invoke(repairPrice);
        }
        if (GameData.Instance != null && GameData.Instance.TrySpendMoney(repairPrice))
        {
            GameData.Instance.RepairHook();
            OnUpgradePurchased?.Invoke();
            Debug.Log("[UpgradeManager]: Perbaikan Kail Berhasil!");
        }
        else
        {
            ShowWarning("Uang Dompet Tidak Cukup!");
        }
    }

    private void ShowWarning(string msg)
    {
        warningText.text = msg;
        // Opsional: Gunakan LeanTween atau Animator untuk hilangkan teks setelah 2 detik
    }
}
