using CarsSystem.Data;

namespace CarsSystem.Interfaces
{
    public interface ICarInformation
    {
        CarDescriptor Descriptor { get; }
    }
}