using GameSession.Warehouses.Data;

namespace GameSession.Warehouses.Interfaces
{
    public interface IWarehouseInformation
    {
        WarehouseDescriptor Descriptor { get; }
        int GetCarsAmount();
    }
}