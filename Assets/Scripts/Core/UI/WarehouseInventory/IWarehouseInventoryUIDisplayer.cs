using System;
using WarehousingSystem.Controllers;

namespace Core.UI.WarehouseInventory
{
    public interface IWarehouseInventoryUIDisplayer
    {
        void Show(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked,
            Action loadCarButtonClicked);
    }
}