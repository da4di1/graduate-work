using System.Collections.Generic;
using UnityEngine;
using WarehousingSystem.Data;

namespace WarehousingSystem.Storages
{
    [CreateAssetMenu(fileName = nameof(WarehousesStorage), menuName = "WarehousesSystem/WarehousesStorage")]
    public class WarehousesStorage : ScriptableObject
    {
        [field: SerializeField] public List<WarehouseDescriptor> WarehouseDescriptors { get; private set; }
    }
}