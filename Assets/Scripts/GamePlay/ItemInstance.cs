using ArusMerah.Data;
using ArusMerah.Interface;
using System;
using UnityEngine;

namespace ArusMerah.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ItemInstance : MonoBehaviour, IHookAble
    {
        [Header("Konfigurasi Data Item")]
        [SerializeField] private ItemTypeSO itemTypeData;

        // Implementasi Interface
        public ItemTypeSO ItemTypeData => itemTypeData;
        public float ItemWeightInKg => itemTypeData != null ? itemTypeData.itemWeightInKg : 1f;
        public float CorrotionDamageToClaw => itemTypeData != null ? itemTypeData.durabilityDamageToClaw : 0f;

        private BoxCollider2D itemCollider2D;
        //private Rigidbody2D itemRigidbody2D;
        private SpriteRenderer itemSpriteRenderer;

       

        private void Awake()
        {
            itemCollider2D = GetComponent<BoxCollider2D>();
            //itemRigidbody2D = GetComponent<Rigidbody2D>();
            itemSpriteRenderer = GetComponent<SpriteRenderer>();

            InitializeVisualSprite();
        }

        public void SetupItemData(ItemTypeSO newItemTypeData)
        {
            itemTypeData = newItemTypeData;
            InitializeVisualSprite();
        }

        public void InitializeVisualSprite()
        {
            if (itemTypeData != null && itemTypeData.itemVisualSprite != null) itemSpriteRenderer.sprite = itemTypeData.itemVisualSprite;
        }

        public void OnCaughtByClaw(Transform clawHookTransform)
        {
            if (TryGetComponent<ItemPatrolMovement>(out var patrolMovement))
            {
                patrolMovement.StopPatrolMovement();
            }

            /*if (itemRigidbody2D != null) itemRigidbody2D.simulated = false;*/ // Matiin simulasi fisika supaya 

            transform.SetParent(clawHookTransform);  // Item nempel ke pengait
            transform.localPosition = Vector3.zero; // Reset posisi item
        }

        public void SellObject()
        {
            if (itemTypeData != null)
            {
                // Tambahkan nilai jual item ke total uang di GameData
                if (GameData.Instance != null)
                {
                    GameData.Instance.AddMoney((int)ItemTypeData.monetaryValue);
                }

                // Tampilkan log monolog nelayan jika ini adalah barang bukti korupsi
                TriggerSpecialNarrativeEffect();
            }

            DestroyObject();
        }

        public void TriggerSpecialNarrativeEffect()
        {
            if (itemTypeData != null && !string.IsNullOrEmpty(itemTypeData.internalMonologueText))
            {
                Debug.Log($"[Monolog Nelayan]: {itemTypeData.internalMonologueText}");
            }
        }

        public void DestroyObject()
        {
            if (ArusMerah.Managers.ObjectPooler.Instance != null )
            {
                ArusMerah.Managers.ObjectPooler.Instance.ReturnToPool(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
