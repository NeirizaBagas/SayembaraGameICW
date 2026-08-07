using System;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warningText; // Tulisan "Uang Tidak Cukup"

    [Header("Upgrade Prices")]
    [SerializeField] private int launchSpeedUpgradePrice = 100;
    [SerializeField] private int retrackSpeedUpgradePrice = 80;
    [SerializeField] private int maxDurabilityUpgradePrice = 150;
    [SerializeField] private int repairPrice = 50;
    [SerializeField] private int price; // Variabel umum untuk menyimpan harga upgrade yang sedang dibeli

    [SerializeField] private float upgradeValue = 0.5f;

    private bool isUpgradingMaxDurability = false; // Flag untuk mengecek apakah upgrade max durability sedang dibeli

    public static Action OnUpgradePurchased; // Event untuk memberi tahu UI agar update setelah upgrade dibeli
    public static Action<int> OnLaunchSpeedPriceUpgraded; // Event khusus untuk upgrade launch speed
    public static Action<int> OnRetrackSpeedPriceUpgraded; // Event khusus untuk upgrade retrack speed
    public static Action<int> OnMaxDurabilityPriceUpgrade;
    public static Action<int> OnRepairPriceUpgraded;

    private void Start()
    {
        OnLaunchSpeedPriceUpgraded?.Invoke(launchSpeedUpgradePrice);
        OnRetrackSpeedPriceUpgraded?.Invoke(retrackSpeedUpgradePrice);
        OnMaxDurabilityPriceUpgrade?.Invoke(maxDurabilityUpgradePrice);
        OnRepairPriceUpgraded?.Invoke(repairPrice);
    }

    // Fungsi ini dipanggil oleh Button Upgrade Speed di Inspector
    public void BuyUpgradeLaunchSpeed()
    {
        if (GameData.Instance.moneyData >= launchSpeedUpgradePrice)
        {
            GameData.Instance.moneyData -= launchSpeedUpgradePrice;
            launchSpeedUpgradePrice *= price; // Naikkan harga untuk upgrade berikutnya
            OnLaunchSpeedPriceUpgraded?.Invoke(launchSpeedUpgradePrice); // Panggil event untuk update harga di UI
            GameData.Instance.UpdateLaunchSpeed(upgradeValue); // Panggil fungsi untuk menambah kecepatan
            Debug.Log("Upgrade launch Berhasil!");
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
            upgradeValue += 2; // Siapkan nilai untuk upgrade berikutnya
        }
        else
        {
            //ShowWarning("Uang Tidak Cukup!");
        }
    }

    public void BuyUpgradeRestrackSpeed()
    {
        if (GameData.Instance.moneyData >= retrackSpeedUpgradePrice)
        {
            GameData.Instance.moneyData -= retrackSpeedUpgradePrice;
            retrackSpeedUpgradePrice *= price; // Naikkan harga untuk upgrade berikutnya
            OnRetrackSpeedPriceUpgraded?.Invoke(retrackSpeedUpgradePrice);
            GameData.Instance.UpdateRetrackSpeed(upgradeValue); // Panggil fungsi untuk menambah kecepatan retrack
            Debug.Log("Upgrade restrack Berhasil!");
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
            upgradeValue += 1; // Siapkan nilai untuk upgrade berikutnya
        }
        else
        {
            //ShowWarning("Uang Tidak Cukup!");
        }
    }

    public void BuyUpgradeMaxDurability()
    {
        if (GameData.Instance.moneyData >= maxDurabilityUpgradePrice)
        {
            GameData.Instance.moneyData -= maxDurabilityUpgradePrice;
            maxDurabilityUpgradePrice *= price; // Naikkan harga untuk upgrade berikutnya
            OnMaxDurabilityPriceUpgrade?.Invoke(maxDurabilityUpgradePrice);
            isUpgradingMaxDurability = true; // Set flag untuk upgrade max durability
            GameData.Instance.UpdateMaxHookDurability(upgradeValue); // Panggil fungsi untuk menambah durabilitas
            //GameData.Instance.launchSpeedLvl++;
            Debug.Log("Upgrade max durability Berhasil!");
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
            upgradeValue += 20;
        }
        else
        {
            //ShowWarning("Uang Tidak Cukup!");
        }
    }

    public void BuyRepair()
    {
        if (isUpgradingMaxDurability)
        {
            repairPrice *= price; // Naikkan harga repair jika sudah upgrade max durability
            isUpgradingMaxDurability = false;
            OnRepairPriceUpgraded?.Invoke(repairPrice);
        }
        if (GameData.Instance.moneyData >= repairPrice)
        {
            GameData.Instance.moneyData -= repairPrice;

            GameData.Instance.currentDurability = GameData.Instance.upgradeAbleMaxHookDurability;
            OnUpgradePurchased?.Invoke(); // Panggil event untuk update UI
        }
        else
        {
            //ShowWarning("Uang Tidak Cukup!");
        }
    }

    //private void ShowWarning(string msg)
    //{
    //    warningText.text = msg;
    //    // Opsional: Gunakan LeanTween atau Animator untuk hilangkan teks setelah 2 detik
    //}
}
