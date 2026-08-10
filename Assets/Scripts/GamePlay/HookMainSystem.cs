using ArusMerah.Data;
using ArusMerah.Gameplay;
using ArusMerah.Interface;
using ArusMerah.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private float hookDelay = 3f; // Waktu cooldown sebelum kail bisa digunakan lagi

    // Hook variables
    // Variabel Status Internal (Private)
    private float currentHookDurability = 100f;
    private Vector3 startPos;
    private LineRenderer ropeLineRenderer;

    // State variables
    private bool isLaunching = false;
    private bool isRetracting = false;
    private bool canHook; // Untuk mengontrol apakah hook bisa digunakan atau tidak (misal saat game over

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
        ropeLineRenderer = hookGameObject.GetComponent<LineRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        FlowManager.OnFlowStateChanged += toggleHook;

        if (FlowManager.instance != null && FlowManager.instance.CurrentFlowState == GameFlowState.GameplayState)
        {
            toggleHook(GameFlowState.GameplayState);
        }
    }

    private void OnDisable()
    {
        inputSystem?.Player.Disable();
        inputSystem.Player.Attack.started -= FireHook;
        FlowManager.OnFlowStateChanged -= toggleHook;
    }

    // Update is called once per frame
    void Update()
    {
        //Logika Meluncur ke Bawah
        if (canHook)
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

    private void toggleHook(GameFlowState gameState)
    {
        if (gameState == GameFlowState.GameplayState)
        {
            // 1. KUNCI BEBAS CRASH: Aktifkan GameObject Player terlebih dahulu jika masih mati!
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            if (ropeLineRenderer != null) ropeLineRenderer.enabled = true;

            // 2. Jalankan Coroutine HANYA JIKA GameObject sudah terbukti aktif di Hierarchy
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RoutineStartCooldown());
            }
            else
            {
                canHook = true; // Fallback jika tidak bisa coroutine
            }

        }
        else if (ropeLineRenderer != null) ropeLineRenderer.enabled = false;
    }

    IEnumerator RoutineStartCooldown()
    {
        canHook = false; // Kunci kail sementara
        yield return new WaitForSeconds(hookDelay); // Tunggu 0.8 detik
        canHook = true; // Buka kunci kail, pemain siap melaut!
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

            // 2. Jual item & tambah uang
            caughtItemObject.SellObject();

            // 3. Trigger Events
            OnHookDurabilityChanged?.Invoke(currentHookDurability / maxHookDurability); // Panggil event untuk memperbarui UI durabilitas & Simpan juga pengurangan ini ke simpanan global GameData
            OnFishSell?.Invoke(); // Panggil event setelah menjual ikan
            OnItemClearedFromSea?.Invoke(); // Trigger event untuk memberitahu LevelSpawner bahwa item telah dihapus dari laut


            caughtItemTransform = null; // Reset caughtItemTransform setelah diproses
            caughtItemObject = null;
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
        canHook = false; // Nonaktifkan hook saat level selesai
    }
}
