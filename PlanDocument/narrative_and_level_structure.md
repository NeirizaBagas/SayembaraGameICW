# Dokumen Alur Narasi, Level Progression & Sistem Ending: ARUS MERAH (Refined Plot)

Dokumen ini merangkum alur cerita terbaru yang berfokus pada **Dilema Personal (Keluarga Sakit)**, penemuan Barang Bukti di Level 9, serta Dual Ending **"Kriminalisasi Iklim (Demo)" vs "Pengkhianat Warga (Jalur Damai/Tambang)"**.

---

## 🗺️ Peta Alur Narasi & Level Progression

```mermaid
graph TD
    L1[Level 1-3: Tahap Infiltrasi] --> L4[Level 4-6: Tahap Gaslighting]
    L4 --> L7[Level 7-8: Ekstraksi & Keracunan Keluarga]
    
    subgraph Tahap 3
        L7 --> L8[Level 8: Keluarga Jatuh Sakit Parah]
        L8 --> L9[Level 9: Otomatis Mendapatkan Barang Bukti]
        L9 --> Post9[Post-Shop Level 9: Diskusi Warga & Ajakan Demo]
    end

    Post9 --> Inter10[Sebelum Level 10: Pihak Tambang Tahu Bukti & Kondisi Keluarga]
    Inter10 --> L10[Level 10: Forced Failure & Tagihan Rumah Sakit Melonjak]

    L10 --> Choice{Pilihan Di Layar Ending}
    
    Choice -- Opsi A: Jalur Demo / Tolak Damai -- EndA[ENDING A: THE RESISTANCE<br>Demo Dibungkam & Kriminalisasi Iklim]
    Choice -- Opsi B: Jalur Damai / Kerja Tambang -- EndB[ENDING B: THE BETRAYAL<br>Keluarga Berobat, tapi Dituduh Pengkhianat Warga]
```

---

## 📖 Rincian Cerita & Plot Point Utama

### 1. Pemicu Forced Failure Level 10 (Keluarga Sakit)
* **Penyebab:** Anggota keluarga nelayan (anak/istri) jatuh sakit parah akibat terus-menerus mengonsumsi ikan termutasi/beracun dan meminum air sumur yang terkontaminasi limbah tambang.
* **Dampak ke Gameplay:** Biaya pengobatan rumah sakit melonjak sangat tinggi (Target Level 10 = **$1.200+**).
* **Kondisi Laut Level 10:** Laut sudah mati (hanya ada ikan tulang $1 & limbah biohazard). Melaut berapa kali pun pasti **gagal mencapai target secara ekonomi**.

### 2. Penemuan Barang Bukti & Interlude Level 9
* **Level 9:** Pemain otomatis mendapatkan **Surat Rahasia Korupsi** di akhir level.
* **Diskusi Warga (Post-Shop Level 9):** Pemain memperlihatkan dokumen itu ke warga desa. Warga geram dan berencana mengadakan aksi demo besar-besaran. Warga mengajak pemain bergabung, tetapi pemain memilih **menahan diri dulu** (karena bimbang memikirkan biaya rumah sakit keluarganya).
* **Interlude Sebelum Level 10:** Pihak pengusaha tambang mengetahui bahwa dokumen rahasia ada di tangan nelayan, sekaligus mengetahui bahwa keluarga nelayan sedang kritis butuh biaya rumah sakit.

---

## 🎭 Rincian Dua Pilihan Ending (#ClimateCorruption)

### 🔴 Opsi A: "Jalur Demo" (*Tolak Damai / The Resistance*)
* **Keputusan Nelayan:** Menolak tawaran damai pihak tambang, memilih berdiri bersama warga desa.
* **Visual & Event:** Gambar kerumunan warga berdemo di depan pintu gerbang tambang/smelter.
* **Hasil Narasi:** Aksi demo tidak didengar, dianggap angin lalu, dan berakhir ricuh. Beberapa warga (termasuk gambaran pemain) ditangkap oleh aparat keamanan.
* **Outro Text:**
  > *"Suaramu dibungkam oleh beton-beton pabrik. Kamu memilih menjaga tanah moyangmu, namun jeruji besi adalah jawaban dari mereka yang merasa memiliki aturan. Di negeri ini, menjaga iklim seringkali dianggap sebagai tindakan kriminal."*

### 🟡 Opsi B: "Jalur Damai / Kerja Tambang" (*The Betrayal & Submission*)
* **Keputusan Nelayan:** Menerima uang damai dari pihak tambang untuk membiayai pengobatan keluarga yang sakit dan menandatangani kontrak kerja smelter.
* **Visual & Event:** Gambar formulir pendaftaran operator smelter PT Global Nickel tertanda tangan di atas meja kayu, dengan latar belakangan warga yang menatap dingin dari luar rumah.
* **Hasil Narasi:** Aksi demo warga batal/gagal karena dokumen kunci diserahkan ke tambang. Pemain mendapatkan uang berobat, tetapi **dicap sebagai PENGKHIANAT** oleh seluruh warga desa.
* **Outro Text:**
  > *"Jaringmu kini menjadi helm proyek. Kamu akhirnya bekerja untuk mesin yang menghancurkan lautmu sendiri. Demi menyembuhkan keluargamu, kamu menjadi pengkhianat bagi warga dan bagian dari rantai yang meracuni sumur air tanahmu. Selamat datang di masa depan yang dijanjikan."*

---

## 💡 Dampak Kualitas Narasi

Konsep perbaikan ini mengubah cerita dari sekadar "pilihan baik vs jahat" menjadi **Dilema Manusiawi yang Sangat Menyentuh (*Heart-wrenching Human Dilemma*)**:
- **Tidak ada jawaban yang 100% bahagia:**
  - Pilih **Demo** ➔ Menjaga kehormatan & lingkungan, tapi ditangkap polisi dan gagal menyelamatkan keluarga.
  - Pilih **Kerja Tambang** ➔ Keluarga berobat selamat, tapi dicap Pengkhianat Warga & merusak alam.
