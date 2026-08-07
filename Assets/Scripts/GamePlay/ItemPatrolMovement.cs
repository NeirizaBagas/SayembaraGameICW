using ArusMerah.Data;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class ItemPatrolMovement : MonoBehaviour
{
    private float swimSpeed = 2f;
    private float leftBoundarySwimX;
    private float rightBoundarySwimX;

    private bool isSwimRight;
    private bool isPatrolActive = false;

    private SpriteRenderer itemSpriteRenderer;

    private void Awake()
    {
        itemSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        isPatrolActive = false; // Hentikan patroli saat item dinonaktifkan
    }

    public void InitializePatrolMovement(ItemTypeSO itemTypeData, Vector3 spawnPosition)
    {
        if (itemTypeData != null && itemTypeData.isSwimmingItem)
        {
            swimSpeed = itemTypeData.swimSpeedInUnitsPerSecond;
            float patrolDistance = itemTypeData.horizontalPatrolDistanceInUnits;

            leftBoundarySwimX = spawnPosition.x - patrolDistance; // Menentukan jarak maksimal patroli sisi kiri
            rightBoundarySwimX = spawnPosition.x + patrolDistance; // Menentukan jarak maksimal patroli sisi kanan

            isSwimRight = Random.value > 0.5f;

            UpdateSpriteFacingDirection();
            isPatrolActive = true;
        }
        else
        {
            isPatrolActive= false; // Item diam(sampah, biohazard, surat)
        }
    }

    private void Update()
    {
        if (!isPatrolActive) return;

        if (isSwimRight)
        {
            transform.Translate(Vector3.right * swimSpeed * Time.deltaTime, Space.World);

            if (transform.position.x >= rightBoundarySwimX)
            {
                isSwimRight= false;
                UpdateSpriteFacingDirection();
            }
        }
        else
        {
            transform.Translate(Vector3.left * swimSpeed * Time.deltaTime, Space.World);

            if (transform.position.x <= leftBoundarySwimX)
            {
                isSwimRight = true;
                UpdateSpriteFacingDirection();
            }
        }
    }

    private void UpdateSpriteFacingDirection()
    {
        if (itemSpriteRenderer != null)
        {
            itemSpriteRenderer.flipX = !isSwimRight;
        }
    }

    public void StopPatrolMovement()
    {
        isPatrolActive = false;
    }
}
