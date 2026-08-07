using UnityEngine;
using System.Collections.Generic;

namespace ArusMerah.Data
{
    // Struct untuk Item Acak (Misal: Ikan & Sampah yang di-spawn acak di kedalaman tertentu)
    [System.Serializable]
    public struct RandomSpawnItem
    {
        public ItemTypeSO itemTypeSO;
        public int minimumSpawnCount;
        public int maximumSpawnCount;
        public float minimumSpawnDepthY; // Batas atas kedalaman (misal: -2.0)
        public float maximumSpawnDepthY; // Batas bawah kedalaman(misal: -8.0)
    }

    [CreateAssetMenu(fileName = "Level_Template_Data", menuName = "ArusMerah/Level")]
    public class LevelDataSO : ScriptableObject
    {
        [Header("1. Informasi Dasar Level")]
        public int levelNumber; // Nomor Level (1 - 10)
        public float targetRevenue; // Target uang/biaya hidup yang harus dicapai
        public float timeLimit; // Batas waktu dalam detik (misal: 60)
        public Color seaWaterColor = Color.cyan; // Warna air laut (Biru -> Keruh -> Merah Karat)

        [Header("2. Narasi & Cutscene")]
        // Cutcene SO

        [Header("1. Informasi Dasar Level")]
        public bool isForcedFailureLevel = false;

        [Header("1. Informasi Dasar Level")]
        public List<RandomSpawnItem> randomSpawnItems;
    }
}
