using ArusMerah.Gameplay;
using ArusMerah.Interface;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookMainSystem : MonoBehaviour
{
    [Header("Referensi Objek")]
    [SerializeField] private GameObject hookGameObject; // Objek Kail/Claw
    [SerializeField] private Transform boatObjTransform; // Transform posisi Perahu

    [Header("Pengaturan Kecepatan & Jarak Kail")]
    [SerializeField] private float launchSpeed = 8f; // Kecepatan meluncur ke bawah
    [SerializeField] private float retrackSpeed = 5f; // Kecepatan menarik ke atas
    [SerializeField] private float maxLaunchDistance = 10f; // Batas jarak maksimum meluncur
    [SerializeField] private float maxHookDurability = 100f; // Durabilitas maksimum kail

    // Hook variables
    // Variabel Status Internal (Private)
    private float currentHookDurability = 100f;
    private Vector3 startPos;
    private LineRenderer ropeLineRenderer;

    // State variables
    private bool isLaunching = false;
    private bool isRetracting = false;
    private bool isActive; // Untuk mengontrol apakah hook bisa digunakan atau tidak (misal saat game over

    private Transform caughtItemTransform = null;
    private IHookAble caughtItemObject = null;

    private float originalRetrackSpeed;
    private float originalMaxLaunchDistance;

    // Script reference
    private InputSystem inputSystem;
    private ClawRotateSystem clawRotateSystem;

    // Action Event untuk Komunikasi ke UI / Manager Lain
    public static Action OnFishSell;
    public static Action<float> OnHookDurabilityChanged;
    public static Action OnItemClearedFromSea; // Event untuk memberitahu LevelSpawner bahwa item telah dihapus dari laut

    private void Awake()
    {
        inputSystem = new InputSystem();
        isActive = true; // Pastikan hook aktif saat game dimulai
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ropeLineRenderer = hookGameObject.GetComponent<LineRenderer>();
        startPos = hookGameObject.transform.position;
        clawRotateSystem = GetComponent<ClawRotateSystem>();

        originalRetrackSpeed = retrackSpeed;
        originalMaxLaunchDistance = maxLaunchDistance;

        RepairHook(); // Set durabilitas awal kail
    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
        inputSystem.Player.Attack.started += FireHook;
        UIManager.OnGamePause += toggleHook;
    }

    private void OnDisable()
    {
        inputSystem?.Player.Disable();
        inputSystem.Player.Attack.started -= FireHook;
        UIManager.OnGamePause -= toggleHook;
    }

    // Update is called once per frame
    void Update()
    {
        //Logika Meluncur ke Bawah
        if (isActive)
        {
            if (isLaunching)
            {
                LaunchHook();
            }
            else if (isRetracting)
            {
                RetractHook();
            }
        }
        UpdateRopeVisual();
    }

    private void toggleHook(bool state)
    {
        isActive = !state;

        if (isActive)
        {
            ropeLineRenderer.enabled = true;
        }
        else ropeLineRenderer.enabled = false;
    }

    private void FireHook(InputAction.CallbackContext context)
    {
        if (!isLaunching && !isRetracting)
        {
            isLaunching = true;
        }
    }

    private void RepairHook()
    {
        currentHookDurability = maxHookDurability;
    }

    private void LaunchHook()
    {
        hookGameObject.transform.Translate(Vector3.down * launchSpeed * Time.deltaTime);
        clawRotateSystem.canRotate = false;
        // Jika mencapai batas jarak, tarik kembali
        if (Vector3.Distance(startPos, hookGameObject.transform.position) >= maxLaunchDistance)
        {
            isLaunching = false;
            isRetracting = true;
        }
    }

    private void RetractHook()
    {
        // Menarik kail kembali ke posisi awal di perahu
        hookGameObject.transform.position = Vector3.MoveTowards(hookGameObject.transform.position, startPos, retrackSpeed * Time.deltaTime);

        // Jika sudah sampai di kapal kembali
        if (Vector3.Distance(hookGameObject.transform.position, startPos) < 0.1f)
        {
            isRetracting = false;
            // Logika jual ikan panggil di sini jika ada caughtItemTransform
            ResetHookAndSellItem();
        }
    }

    private void UpdateRopeVisual()
    {
        if (ropeLineRenderer != null && boatObjTransform != null)
        {
            ropeLineRenderer.SetPosition(0, boatObjTransform.position);
            ropeLineRenderer.SetPosition(1, hookGameObject.transform.position);
        }

    }

    private void ResetHookAndSellItem()
    {
        hookGameObject.transform.position = startPos;
        if (caughtItemObject != null)
        {
            // Total Kerusakan = (Berat Item dalam Kg) + (Kerusakan Korosi Kimia)
            float totalDurabilityDamage = caughtItemObject.ItemWeightInKg + caughtItemObject.CorrotionDamageToClaw;

            // Pengurangan durabilitas di variabel lokal HookMainSystem
            currentHookDurability -= totalDurabilityDamage;
            currentHookDurability = Math.Max(0, currentHookDurability);

            // Simpan juga pengurangan ini ke simpanan global GameData
            if (GameData.Instance != null) GameData.Instance.ApplyDurabilityDamage(totalDurabilityDamage);

            // 2. Jual item & tambah uang
            caughtItemObject.SellObject();

            // 3. Trigger Events
            OnHookDurabilityChanged?.Invoke(currentHookDurability / maxHookDurability); // Panggil event untuk memperbarui UI durabilitas
            OnFishSell?.Invoke(); // Panggil event setelah menjual ikan
            OnItemClearedFromSea?.Invoke(); // Trigger event untuk memberitahu LevelSpawner bahwa item telah dihapus dari laut


            caughtItemTransform = null; // Reset caughtItemTransform setelah diproses
            caughtItemObject = null;

            Debug.Log("Tes");
            // Sistem skor atau harga dari hasil tangkapan
        }
        ResetStat();
    }

    private void ResetStat() // Reset kecepatan tarik pas selesai menarik
    {

        clawRotateSystem.canRotate = true; // Aktifkan kembali rotasi setelah reset

        retrackSpeed = originalRetrackSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLaunching && !isRetracting)
        {
            IHookAble collidedTakeableItem = collision.GetComponent<IHookAble>();

            if (collidedTakeableItem != null)
            {
                caughtItemObject = collidedTakeableItem;
                caughtItemTransform = collision.transform;

                if (collidedTakeableItem != null)
                {
                    // Membuat item mengikuti objek claw/hook
                    collidedTakeableItem.OnCaughtByClaw(hookGameObject.transform);
                }

                isLaunching = false;
                isRetracting = true;

                float itemWeightInKg = collidedTakeableItem.ItemWeightInKg;

                if (itemWeightInKg > 0) retrackSpeed = retrackSpeed / itemWeightInKg;
            }
        }

        if (collision.CompareTag("BatasMap"))
        {
            isLaunching = false;
            isRetracting = true;
        }
    }

    private void HandleLevelEnded(bool isGameCompleted)
    {
        isActive = false; // Nonaktifkan hook saat level selesai
    }
}
