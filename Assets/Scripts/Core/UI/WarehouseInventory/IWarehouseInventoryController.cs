using System;
using WarehousingSystem.Controllers;

namespace Core.UI.WarehouseInventory
{
    public interface IWarehouseInventoryController
    {
        bool IsWarehouseInventoryUIShown { get; }
        event Action WarehouseInventoryAppeared;
        event Action WarehouseInventoryDisappeared;
        void ShowWarehouseInventory(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked,
            Action loadCarButtonClicked);
    }
}