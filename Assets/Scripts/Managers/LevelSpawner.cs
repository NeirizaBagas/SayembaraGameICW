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

    public void SpawnObjectsForLevel(LevelDataSO currentLevelDataSO)
    {
        ClearAllSpawnedObjects();

        if (currentLevelDataSO == null) return;

        objekToSpawn = 0;

        foreach (RandomSpawnItem randomSpawn in currentLevelDataSO.randomSpawnItems)
        {
            if (randomSpawn.itemTypeSO == null) continue;

            for (int i = 0; i < randomSpawn.spawnCount; i++)
            {
                objekToSpawn++;
                Vector3 randomSpawnPosition = GenerateRandomWorldPosition(randomSpawn.minimumSpawnDepthY, randomSpawn.maximumSpawnDepthY);
                SpawnSingleItemFromPool(randomSpawn.itemTypeSO, randomSpawnPosition);
            }
        }

        if (levelManager != null)
        {
            levelManager.HandleItemsSpawn(objekToSpawn);
        }
    }

    private void SpawnSingleItemFromPool(ItemTypeSO itemTypeSO, Vector3 spawnWorldPosition)
    {
        if (ObjectPooler.Instance == null) return;

        GameObject spawnedObject = ObjectPooler.Instance.SpawnFromPool(itemTypeSO, spawnWorldPosition, Quaternion.identity);

        if (spawnedObject != null)
        {
            spawnedObject.transform.SetParent(spawnedItemParentContainer);
            currentlyActiveSpawnedItemsList.Add(spawnedObject);

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
