using System.Runtime.CompilerServices;
using UnityEngine;

public class GameData: MonoBehaviour
{
    public static GameData Instance { get; private set; } // Singleton agar mudah dipanggil

    [Header("Game Data")]
    public int moneyData;
    public float upgradeAbleLaunchSpeed;
    public float upgradeAbleRetrackSpeed;
    public float upgradeAbleMaxHookDurability;
    public float currentDurability;

    private float baseLaunchSpeed = 8f;
    private float baseRetrackSpeed = 5f;
    private float baseMaxHookDurability = 100f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Pastikan hanya ada satu instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Agar tetap ada saat ganti scene
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeAbleLaunchSpeed = baseLaunchSpeed;
        upgradeAbleRetrackSpeed = baseRetrackSpeed;
        upgradeAbleMaxHookDurability = baseMaxHookDurability;
        currentDurability = upgradeAbleMaxHookDurability;
    }

    // Fungsi Menambah Uang dari Hasil Penjualan Item
    public void AddMoney(int amountToAdd)
    {
        moneyData += amountToAdd;
        Debug.Log($"[GameData]: Uang bertambah ${amountToAdd}. Total Uang Sekarang: ${moneyData}");
    }

    // Fungsi Pengurangan Durabilitas Kail
    public void ApplyDurabilityDamage(float damageAmount)
    {
        currentDurability -= damageAmount;
        currentDurability = Mathf.Max(0, currentDurability);
    }

    // Fungsi Upgrade yang Dipanggil di UI Shop
    public void UpdateLaunchSpeed(float amountUpgrade) => upgradeAbleLaunchSpeed *= amountUpgrade;

    public void UpdateRetrackSpeed(float amountUpgrade) => upgradeAbleRetrackSpeed *= amountUpgrade;

    public void UpdateMaxHookDurability(float amountUpgrade) => upgradeAbleMaxHookDurability *= amountUpgrade;

    public void RepairHook() => currentDurability = upgradeAbleMaxHookDurability; // Set durabilitas saat diperbaiki
}
