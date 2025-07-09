using System;
using GameSession.Cars.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSession.UI.WarehouseInventory.Data
{ 
    [Serializable]
    public class CarUIDescriptor
    {
        [field: SerializeField] public CarType CarType { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Amount { get; private set; }
        [field: SerializeField] public Image Icon { get; private set; }
    }
}