using NUnit.Framework.Interfaces;
using UnityEngine;

public class ItemData : MonoBehaviour, ITakeAbleObject
{
    public string itemName;
    public int itemPrice;
    public float itemWeight;
    public float corotion;
    public bool isSpecialEvidence;

    public void SellObject()
    {
        GameData.moneyData += itemPrice;
        DestroyObject();
    }

    public void DestroyObject()
    {
        Debug.Log("SelfDestruct");
        Destroy(gameObject);
    }
}
