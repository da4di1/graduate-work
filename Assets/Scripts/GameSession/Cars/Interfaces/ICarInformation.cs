using GameSession.Cars.Data;

namespace GameSession.Cars.Interfaces
{
    public interface ICarInformation
    {
        CarDescriptor Descriptor { get; }
    }
}