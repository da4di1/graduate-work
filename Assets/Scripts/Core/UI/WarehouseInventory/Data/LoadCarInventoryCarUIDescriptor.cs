using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.WarehouseInventory.Data
{
    [Serializable]
    public class LoadCarInventoryCarUIDescriptor : CarUIDescriptor
    {
        [field: SerializeField] public TextMeshProUGUI Capacity { get; private set; }
        [field: SerializeField] public Button PickButton { get; private set; }
    }
}