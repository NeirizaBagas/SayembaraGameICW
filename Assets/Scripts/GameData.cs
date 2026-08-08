using System.Runtime.CompilerServices;
using UnityEngine;

namespace ArusMerah.Data
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance { get; private set; } // Singleton agar mudah dipanggil

        [Header("Money Data")] 
        public int walletBalance;  // Uang bersih di dompet (dipakai di Shop & dibawa antar level)
        public int grossEarningsToday; // Uang kotor hasil pancingan hari ini (direset tiap level)
        public int walletSnapshotAtStart; // Catatan saldo dompet di awal hari (untuk Rollback jika gagal quota)

        [Header("Upgradeable Hook Data")]
        public float upgradeAbleLaunchSpeed;
        public float upgradeAbleRetrackSpeed;
        public float upgradeAbleMaxHookDurability;
        public float currentDurability;

        [Header("Base Hook Data")]
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

            // Inisialisasi keuangan awal
            //walletBalance = 0;
            grossEarningsToday = 0;
            walletSnapshotAtStart = 0;
        }

        // Fungsi untuk mencatat snapshot di setiap level/hari baru
        // Dipanggil oleh FlowManager di awal setiap level.
        public void RecordStartOfDaySnapshot() 
        {
            walletSnapshotAtStart = walletBalance;
            grossEarningsToday = 0;
            Debug.Log($"[GameData]: Snapshot Awal Hari Dicatat. Saldo Dompet: ${walletBalance}");
        }

        // Menambahkan ke uang kotor hari ini, BUKAN langsung ke dompet.
        // Fungsi ini dipanggil saat item dijual di HookMainSystem.cs
        public void AddGrossEarnings(int amountToAdd) 
        {
            grossEarningsToday += amountToAdd;
            Debug.Log($"[GameData]: Uang bertambah ${amountToAdd}. Total Uang Sekarang: ${grossEarningsToday}");
        }

        // Dipanggil oleh Panel Result jika pemain LULUS quota (grossEarningsToday >= targetRevenue).
        // Menghitung sisa keuntungan bersih dan menambahkannya ke dompet.
        public void ApplyQuotaDeductionAndSaveProfit(int targetRevenueQuota)
        {
            int netProfit = grossEarningsToday - targetRevenueQuota; // Hitung keuntungan bersih setelah dikurangi quota target harian
            walletBalance += walletSnapshotAtStart + netProfit; // Tambahkan keuntungan bersih ke dompet
            Debug.Log($"[GameData]: Quota ${targetRevenueQuota} dipotong. Profit Bersih: +${netProfit}. Saldo Dompet Baru: ${walletBalance}");
        }

        // Dipanggil oleh FlowManager saat pemain menekan tombol Retry (gagal quota).
        // Mengembalikan saldo dompet ke catatan awal hari.
        public void RollbackMoneyToStartOfDay()
        {
            walletBalance = walletSnapshotAtStart;
            grossEarningsToday = 0; // Reset uang kotor hari ini
            Debug.Log($"[GameData]: Rollback Saldo Dompet ke Awal Hari: ${walletBalance}");
        }

        // Dipanggil oleh UpgradeManager saat pemain membeli upgrade di Shop.
        // Mengurangi uang dompet bersih.
        public bool TrySpendMoney(int cost)
        {
            if (walletBalance >= cost)
            {
                walletBalance -= cost;
                Debug.Log($"[GameData]: Belanja -${cost}. Sisa Dompet: ${walletBalance}");
                return true;
            }
            Debug.Log($"[GameData]: Uang tidak cukup! Butuh ${cost}, Saldo: ${walletBalance}");
            return false;
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
}
