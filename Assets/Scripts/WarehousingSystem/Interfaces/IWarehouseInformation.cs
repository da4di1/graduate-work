using WarehousingSystem.Data;

namespace WarehousingSystem.Interfaces
{
    public interface IWarehouseInformation
    {
        WarehouseDescriptor Descriptor { get; }
        int GetCarsAmount();
    }
}