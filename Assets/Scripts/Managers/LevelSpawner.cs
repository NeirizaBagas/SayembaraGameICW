using UnityEngine;
using ArusMerah.Data;
using ArusMerah.Managers;
using System.Collections.Generic;
using ArusMerah.Gameplay;
using System;
public class LevelSpawner : MonoBehaviour
{
    public static LevelSpawner Instance { get; private set; }

    [Header("Container Objek Laut")]
    [SerializeField] private Transform spawnedItemParentContainer;

    [Header("Private Value")]
    private int objekToSpawn;
    private LevelManager levelManager;

    private List<GameObject> currentlyActiveSpawnedItemsList = new List<GameObject>();

    [Header("Action")]
    public static Action<int> OnItemSpawned;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        levelManager = GetComponent<LevelManager>();
    }

    private void OnEnable()
    {
        FlowManager.OnLevelDataLoaded += SpawnObjectsForLevel;
    }

    private void OnDisable()
    {
        FlowManager.OnLevelDataLoaded -= SpawnObjectsForLevel;
    }

    // Memuat dan memunculkan seluruh objek di laut berdasarkan LevelDataSO
    public void SpawnObjectsForLevel(LevelDataSO currentLevelDataSO)
    {
        // Bersihin kondisi dari level sebelumnya
        ClearAllSpawnedObjects();

        if (currentLevelDataSO == null) return;

        foreach (RandomSpawnItem randomSpawn in currentLevelDataSO.randomSpawnItems)
        {
            if (randomSpawn.itemTypeSO != null)
            {

                int randomSpawnCount = UnityEngine.Random.Range(randomSpawn.minimumSpawnCount, randomSpawn.maximumSpawnCount + 1);
                Debug.Log($"Spawning {randomSpawnCount} of {randomSpawn.itemTypeSO.itemDisplayName} for Level {currentLevelDataSO.levelNumber}");

                for (int i = 0; i < randomSpawnCount; i++)
                {
                    objekToSpawn++;

                    if (objekToSpawn == randomSpawnCount)
                    {
                        LevelManager.Instance.HandleItemsSpawn(objekToSpawn);
                    }

                    Vector3 randomSpawnPosition = GenerateRandomWorldPosition(randomSpawn.minimumSpawnDepthY, randomSpawn.maximumSpawnDepthY);

                    SpawnSingleItemFromPool(randomSpawn.itemTypeSO, randomSpawnPosition);

                    
                }


            }
        }
    }

    private void SpawnSingleItemFromPool(ItemTypeSO itemTypeSO, Vector3 spawnWorldPosition)
    {
        if (ObjectPooler.Instance == null) return;

        string poolTagKey = itemTypeSO.itemDisplayName;
        GameObject spawnedObject = ObjectPooler.Instance.SpawnFromPool(poolTagKey, spawnWorldPosition, Quaternion.identity);

        if (spawnedObject != null)
        {
            spawnedObject.transform.SetParent(spawnedItemParentContainer);
            currentlyActiveSpawnedItemsList.Add(spawnedObject);

            // Inisialisasi Data Item
            if (spawnedObject.TryGetComponent<ItemInstance>(out var itemInstance))
            {
                itemInstance.SetupItemData(itemTypeSO);
            }

            // Inisialisasi Pergerakan Patroli (jika ini adalah ikan)
            if (spawnedObject.TryGetComponent<ItemPatrolMovement>(out var patrolMovement))
            {
                patrolMovement.InitializePatrolMovement(itemTypeSO, spawnWorldPosition);
            }
        }
    }

    private Vector3 GenerateRandomWorldPosition(float minimumDepthY, float maximumDepthY)
    {
        // Batas horizontal (X) layar laut (misal: -7.0 sampai 7.0)
        float randomPositionX = UnityEngine.Random.Range(-7f, 7f);
        float randomPositionY = UnityEngine.Random.Range(minimumDepthY, maximumDepthY);
        return new Vector3(randomPositionX, randomPositionY, 0f);
    }
    public void ClearAllSpawnedObjects()
    {
        foreach (GameObject activeItem in currentlyActiveSpawnedItemsList)
        {
            if (activeItem != null && activeItem.activeSelf)
            {
                ObjectPooler.Instance.ReturnToPool(activeItem);
            }
        }
        currentlyActiveSpawnedItemsList.Clear();
    }
}
