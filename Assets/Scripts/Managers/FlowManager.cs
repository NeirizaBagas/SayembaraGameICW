using ArusMerah.Data;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    [SerializeField] private LevelDataSO testLevelDataSO;
    private void Start()
    {
        // Langsung spawn objek level saat tombol Play ditekan
        if (LevelSpawner.Instance != null && testLevelDataSO != null)
        {
            LevelSpawner.Instance.SpawnObjectsForLevel(testLevelDataSO);
            Debug.Log("[Test Launcher]: Sukses spawn ikan berenang dan limbah!");
        }
    }
}
