using System.Collections.Generic;
using GameSession.Warehouses.Data;
using UnityEngine;

namespace GameSession.Warehouses.Storages
{
    [CreateAssetMenu(fileName = nameof(WarehousesStorage), menuName = "WarehousesSystem/WarehousesStorage")]
    public class WarehousesStorage : ScriptableObject
    {
        [field: SerializeField] public List<WarehouseDescriptor> WarehouseDescriptors { get; private set; }
    }
}