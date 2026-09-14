# 🔬 Selective TDD Strategy Document: ARUS MERAH

Strategi pengujian proyek **ARUS MERAH** menerapkan prinsip **Selective TDD (Test-Driven Development Terseleksi)**:
* **Automated Unit/Integration TDD:** Diterapkan HANYA pada *Pure Data & Business Logic* (Domain Layer).
* **Manual Playtest & Visual Inspection:** Diterapkan pada *Presentation / UI / Graphic Layer*.

---

## 🎯 1. Arsitektur Pemisahan Layer (Separation of Concerns)

```
 ┌──────────────────────────────────────────────────────────┐
 │ 1. DOMAIN LAYER (Pure Business Logic & Math)             │
 │    • GameData (Wallet Balance, Quota Deduction, Rollback)│
 │    • LevelManager (Session Timer, Cleared Sea Check)     │
 │    • Hook Logic (Weight vs Retract Speed Math)           │
 └────────────────────────────┬─────────────────────────────┘
                              │ (Siarkan C# Action Events)
                              ▼
 ┌──────────────────────────────────────────────────────────┐
 │ 2. PRESENTATION / VISUAL LAYER                           │
 │    • UIManager, UIPanel, & Panel Controllers             │
 │    • LineRenderer Visual Tali Kail                       │
 │    • CanvasGroup Fade In/Out Transitions                 │
 └──────────────────────────────────────────────────────────┘
```

---

## 🧪 2. Selective TDD Matriks Pengujian

### A. AUTOMATED TDD (Pure Data & Domain Logic Layer)
Fungsi-fungsi logika berikut diuji menggunakan C# NUnit / Unity Test Runner tanpa bergantung pada GameObject/UI:

| Modul Domain Logic | Skenario Tes Otomatis (TDD Spec) | Kriteria Lulus (Assertion) |
| :--- | :--- | :--- |
| **`GameData.RecordStartOfDaySnapshot`** | Catat snapshot saldo sebelum melaut | `walletSnapshotAtStart == walletBalance` & `grossEarningsToday == 0` |
| **`GameData.AddGrossEarnings`** | Pemain menjual ikan bernilai $50 | `grossEarningsToday` bertambah $50 (saldo dompet belum berubah) |
| **`GameData.ApplyQuotaDeductionAndSaveProfit`** | Gross: $150, Quota: $100 (Lulus Quota) | Saldo dompet bertambah keuntungannya (`+ $50`) |
| **`GameData.RollbackMoneyToStartOfDay`** | Gross: $40, Quota: $100 (Gagal Quota / Retry) | `walletBalance == walletSnapshotAtStart` & `grossEarningsToday == 0` |
| **`GameData.TrySpendMoney`** | Beli upgrade $80 saat dompet $100 vs $50 | Return `true` & sisa $20 saat $100; Return `false` saat $50 |
| **`HookMainSystem.ResetHookAndSellItem`** | Hitung damage durabilitas = `ItemWeight` + `CorrotionDamage` | `currentHookDurability` berkurang tepat sebesar total damage |
| **`LevelManager.HandleItemCollected`** | Tangkap seluruh 8 item di laut sebelum 60s habis | Mentrigger `EvaluateLevelEnd()` instan tanpa menunggu timer |

---

### B. MANUAL PLAYTEST & VISUAL INSPECTION (Presentation Layer)
Visual dan antarmuka UI berikut diverifikasi secara manual lewat Unity Editor / Playtest:

| Komponen Visual UI | Aspek yang Diverifikasi Secara Manual (Playtest Check) |
| :--- | :--- |
| **`UIManager.TransitionToTargetCanvas`** | Pastikan saat berpindah layar, HANYA canvas target yang `alpha = 1`, sedangkan canvas lain `alpha = 0` dan `SetActive(false)`. |
| **`HookMainSystem.LineRenderer`** | Visual tali perahu terhubung mulus dari posisi perahu ke ujung kail saat meluncur. |
| **`CutsceneUIController`** | Teks `headlineTitleText` dan `mainBodyNarrationText` tampil tebal & mudah dibaca di dalam Chatbox. |
| **`ItemPatrolMovement`** | Sprite ikan membalik secara horizontal (Flip X) saat mencapai batas renang `leftBoundarySwimX` dan `rightBoundarySwimX`. |
| **`EventSystem UI Input Block`** | Mengeklik tombol Retry di UI **TIDAK TEMBUS** membuat kail meluncur di frame yang sama. |

---

## 💡 3. Ringkasan Keuntungan Strategi Selective TDD

1. **Efisiensi Waktu Murni:** Mengeliminasi waktu terbuang akibat mencoba membuat *unit test UI/Animation* yang rumit, lalu memfokuskan tes otomatis 100% pada logika matematika keuangan & matematika permainan.
2. **NOL Bug Keuangan:** Logika potong quota, rollback uang saat kalah, dan transaksi toko dijamin 100% presisi dan bebas bug duplikasi uang.
3. **Refactoring Aman:** Komponen logika dapat diubah di masa depan tanpa takut merusak aturan main dasar.
