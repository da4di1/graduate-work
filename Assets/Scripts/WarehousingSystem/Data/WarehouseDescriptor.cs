using System;
using UnityEngine;
using WarehousingSystem.Enums;

namespace WarehousingSystem.Data
{
    [Serializable]
    public class WarehouseDescriptor
    {
        [field: SerializeField] public WarehouseId Id { get; private set; }
        [field: SerializeField] public WarehouseType WarehouseType { get; private set; }
        [field: SerializeField] public float Price { get; private set; }
        [field: SerializeField] public float ProductCapacity { get; private set; }
        [field: SerializeField] public float CarCapacity { get; private set; }
    }
}