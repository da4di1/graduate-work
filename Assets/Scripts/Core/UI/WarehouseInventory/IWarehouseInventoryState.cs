using System;

namespace Core.UI.WarehouseInventory
{
    public interface IWarehouseInventoryState
    {
        bool IsWarehouseInventoryUIShown { get; }
        event Action WarehouseInventoryAppeared;
        event Action WarehouseInventoryDisappeared;
    }
}