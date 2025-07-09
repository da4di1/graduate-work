using System;
using GameSession.Warehouses.Controllers;

namespace GameSession.UI.WarehouseInventory.Interfaces
{
    public interface IWarehouseInventoryUIDisplayer
    {
        void Show(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked, Action loadCarButtonClicked);
    }
}