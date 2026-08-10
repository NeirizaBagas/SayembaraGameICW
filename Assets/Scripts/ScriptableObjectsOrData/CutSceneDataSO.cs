using UnityEngine;

namespace ArusMerah.Data
{
    public enum CutsceneDisplayType
    {
        NewspaperHeadline,     // Berita Koran di awal level (Lvl 1, 4, 7)
        InterludeDialogue,     // Selebaran Loker (Lvl 8) / Ajakan Demo (Lvl 9)
        EndingChoiceDialog     // Layar Pilihan Dual Ending (Khusus Level 10)
    }

    [CreateAssetMenu(fileName = "NewCutsceneData", menuName = "ArusMerah/Cutscene Data")]
    public class CutSceneDataSO : ScriptableObject
    {
        [Header("Tipe Tampilan Cutscene")]
        public CutsceneDisplayType displayType;

        [Header("Visual & Teks Utama")]
        public Sprite visualBackgroundSprite;        // Visual Gambar Koran / Selebaran / Formulir
        public string headlineTitleText;             // Judul Headline Berita

        [TextArea(4, 8)]
        public string mainBodyNarrationText;         // Isi Teks Berita / Dialog Narasi

        [Header("Khusus Layar Pilihan Ending (Level 10)")]
        public bool isEndingChoiceCutscene = false;  // Centang TRUE khusus Cutscene Ending Level 10
        public string optionAButtonText = "Ikut Berdemo";
        public string optionBButtonText = "Daftar Kerja Tambang";

        [Header("Outro Narasi Hasil Pilihan Ending")]
        [TextArea(3, 6)]
        public string optionAOutroText;              // Teks Outro jika memilih Demo (Kriminalisasi)

        [TextArea(3, 6)]
        public string optionBOutroText;              // Teks Outro jika memilih Kerja Tambang (Pengkhianat)
    }
}