using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookMainSystem : MonoBehaviour
{
    [SerializeField] private GameObject hookObj;
    [SerializeField] private Transform boatObj;
    [SerializeField] private float launchSpeed = 8f;
    [SerializeField] private float retrackSpeed = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float maxHookDurability = 100f;

    private float currentHookDurability = 100f;
    private Vector3 startPos;
    private LineRenderer lineRenderer;
    private bool isLaunching = false;
    private bool isRetracting = false;
    private Transform caughtObject = null;
    private InputSystem inputSystem;
    private ClawRotateSystem clawRotateSystem;
    private ItemData itemData;
    private float originalRetrackSpeed;
    private float originalMaxDistance;

    public static Action OnFishSell;
    public static Action<float> OnHookDurabilityChanged;

    private void Awake()
    {
        inputSystem = new InputSystem();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = hookObj.GetComponent<LineRenderer>();
        startPos = hookObj.transform.position;
        clawRotateSystem = GetComponent<ClawRotateSystem>();
        originalRetrackSpeed = retrackSpeed;
        originalMaxDistance = maxDistance;
    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
        inputSystem.Player.Attack.started += FireHook;
    }

    private void OnDisable()
    {
        inputSystem?.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Logika Meluncur ke Bawah
        if (isLaunching)
        {
            clawRotateSystem.canRotate = false; // Nonaktifkan rotasi saat meluncur
            LaunchHook();
        }
        else if (isRetracting) //Logika Menarik Kembali ke Atas
        {
            RetractHook();
        }
        UpdateRopeVisual();
    }

    private void FireHook(InputAction.CallbackContext context)
    {
        if (!isLaunching && !isRetracting)
        {
            isLaunching = true;
        }
    }

    private void LaunchHook()
    {
        hookObj.transform.Translate(Vector3.down * launchSpeed * Time.deltaTime);
        // Jika mencapai batas jarak, tarik kembali
        if (Vector3.Distance(startPos, hookObj.transform.position) >= maxDistance)
        {
            isLaunching = false;
            isRetracting = true;
        }
    }

    private void RetractHook()
    {
        hookObj.transform.position = Vector3.MoveTowards(hookObj.transform.position, startPos, retrackSpeed * Time.deltaTime);

        // Jika sudah sampai di kapal kembali
        if (Vector3.Distance(hookObj.transform.position, startPos) < 0.1f)
        {
            isRetracting = false;
            // Logika jual ikan panggil di sini jika ada caughtObject
            ResetHook();
        }
    }

    private void UpdateRopeVisual()
    {
        lineRenderer.SetPosition(0, boatObj.position);
        lineRenderer.SetPosition(1, hookObj.transform.position);
    }

    private void ResetHook()
    {
        hookObj.transform.position = startPos;
        foreach (Transform t in hookObj.transform)
        {

            if (itemData != null)
            {
                itemData.SellObject(); // Jual objek yang tertangkap
                currentHookDurability -= itemData.itemWeight + itemData.corotion; // Contoh pengurangan durabilitas berdasarkan berat item
                OnFishSell?.Invoke(); // Panggil event setelah menjual ikan
                OnHookDurabilityChanged?.Invoke(currentHookDurability / maxHookDurability); // Panggil event untuk memperbarui UI durabilitas

                // Sistem skor atau harga dari hasil tangkapan
                itemData = null; // Reset itemData setelah diproses
            }
            caughtObject = null; // Reset caughtObject setelah diproses
        }
        clawRotateSystem.canRotate = true; // Aktifkan kembali rotasi setelah reset
        ResetStat();
    }


    private void ResetStat()
    {
        retrackSpeed = originalRetrackSpeed;
        maxDistance = originalMaxDistance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLaunching && !isRetracting)
        {
            itemData = collision.GetComponent<ItemData>();

            if (itemData != null)
            {
                caughtObject = collision.transform; // Simpan objek yang tertangkap
                isLaunching = false;
                isRetracting = true;

                // referensi item ke kail
                collision.transform.SetParent(hookObj.transform); // Pasangkan objek ke kapal
                collision.transform.localPosition = Vector3.zero; // Atur posisi objek ke tengah hook

                // Sesuaikan kecepatan menarik berdasarkan berat item
                retrackSpeed = retrackSpeed / itemData.itemWeight; // Simpan kecepatan menarik saat ini
            }
        }

        if (collision.CompareTag("BatasMap"))
        {
            isLaunching = false;
            isRetracting = true;
        }
    }
}
