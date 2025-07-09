using GameSession.Warehouses.Interfaces;

namespace GameSession.PlayerAccount
{
    public interface IPlayerWarehousesController
    {
        void AddWarehouse(IWarehouseInformation warehouse);
        void RemoveWarehouse(IWarehouseInformation warehouse);
    }
}