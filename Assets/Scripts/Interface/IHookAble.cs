using UnityEngine;
using ArusMerah.Data;

namespace ArusMerah.Interface
{
    public interface IHookAble
    {
        ItemTypeSO ItemTypeData { get; }
        float ItemWeightInKg { get; }
        float CorrotionDamageToClaw { get; }

        void OnCaughtByClaw(Transform clawHookTransform);
        void SellObject();
        void DestroyObject();
        
    }
}
