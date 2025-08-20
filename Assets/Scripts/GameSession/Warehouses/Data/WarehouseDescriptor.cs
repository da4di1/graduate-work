using System;
using GameSession.Warehouses.Enums;
using UnityEngine;

namespace GameSession.Warehouses.Data
{
    [Serializable]
    public class WarehouseDescriptor
    {
        [field: SerializeField] public WarehouseID ID { get; private set; }
        [field: SerializeField] public float Cost { get; private set; }
        [field: SerializeField] public float SalePrice { get; private set; }
        [field: SerializeField] public float ProductCapacity { get; private set; }
        [field: SerializeField] public float CarCapacity { get; private set; }
    }
}