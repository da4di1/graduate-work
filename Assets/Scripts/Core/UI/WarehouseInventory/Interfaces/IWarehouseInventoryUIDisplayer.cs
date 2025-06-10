using System;
using WarehousingSystem.Controllers;

namespace Core.UI.WarehouseInventory.Interfaces
{
    public interface IWarehouseInventoryUIDisplayer
    {
        void Show(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked,
            Action loadCarButtonClicked);
    }
}