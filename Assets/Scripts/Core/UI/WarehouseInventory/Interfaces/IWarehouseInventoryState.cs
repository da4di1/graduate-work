using System;

namespace Core.UI.WarehouseInventory.Interfaces
{
    public interface IWarehouseInventoryState
    {
        bool IsWarehouseInventoryUIShown { get; }
        event Action WarehouseInventoryAppeared;
        event Action WarehouseInventoryDisappeared;
    }
}