using System;
using WarehousingSystem.Controllers;

namespace Core.UI.WarehouseInventory
{
    public interface IWarehouseInventoryUIDisplayer
    {
        void ShowWarehouseInventory(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked,
            Action loadCarButtonClicked);
    }
}