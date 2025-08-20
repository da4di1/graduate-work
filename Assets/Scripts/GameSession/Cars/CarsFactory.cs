using System.Collections.Generic;
using GameSession.Cars.Controllers;
using GameSession.Cars.Data;
using GameSession.Cars.Enums;
using UnityEngine;

namespace GameSession.Cars
{
    public class CarsFactory
    {
        private readonly List<CarDescriptor> _carDescriptors;


        public CarsFactory(List<CarDescriptor> carDescriptors)
        {
            _carDescriptors = carDescriptors;
        }

        public CarEntity CreateCar(CarType carType, Vector3[] pathPositions)
        {
            CarEntity carEntity = null;
            CarDescriptor carDescriptor = _carDescriptors.Find(descriptor => descriptor.Type == carType);
            if (carDescriptor != null)
            {
                carEntity = new CarEntity(carDescriptor, pathPositions);
            }
            return carEntity;
        }
    }
}