using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.WarehouseInventory.Data
{
    [Serializable]
    public class WarehouseInventoryCarUIDescriptor : CarUIDescriptor
    {
        [field: SerializeField] public TextMeshProUGUI Cost { get; private set; }
        [field: SerializeField] public TextMeshProUGUI SalePrice { get; private set; }
        [field: SerializeField] public Button BuyButton { get; private set; }
        [field: SerializeField] public Button SellButton { get; private set; }
    }
}