using UnityEngine;

namespace ArusMerah.Data
{
    public enum ItemCategoryType
    {
        bigFish, // Item ikan kualitas unggul
        mutatedFish, // Item ikan kurang berkualitas karna tercemar
        smallFish, // Item ikan kualitas umum 
        corrosiveMiningWaste, // Item sampah pertambangan
        corruptionEvidence // Item sampah tambang yang bersifat korosif
    }

    [CreateAssetMenu(fileName = "NewItemTypeData", menuName = "ArusMerah/Item Type Data")]
    public class ItemTypeSO : ScriptableObject
    {
        [Header("Basic Item Information")]
        public string itemDisplayName; // Nama item di ui
        public ItemCategoryType categoryType; // Jenis item
        public GameObject itemPrefab;

        [Header("Gameplay Statistic")]
        public int monetaryValue; // Harga jual item
        public float itemWeightInKg; // Berat item dalam kilogram
        public float durabilityDamageToClaw; // Jumlah kerusakan yang diberikan ke pengait

        [Header("Internal Dialog & Narrative")]
        [TextArea(3, 6)]
        public string internalMonologueText; // Teks ketika pemain atau nelayan mendapatkan spesial item yang diangkat

        [Header("Pengaturan Pergerakan Renang (Ikan)")]
        public bool isSwimmingItem = false;                 // Centang TRUE jika item ini ikan yang berenang
        public float swimSpeedInUnitsPerSecond = 2f;         // Kecepatan renang ikan
        public float horizontalPatrolDistanceInUnits = 3f;   // Jarak jangkauan renang bolak-balik (kiri-kanan)
    }
}
