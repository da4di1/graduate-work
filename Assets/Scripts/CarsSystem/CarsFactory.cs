using System.Collections.Generic;
using CarsSystem.Controllers;
using CarsSystem.Data;
using CarsSystem.Enums;
using UnityEngine;

namespace CarsSystem
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
            CarDescriptor carDescriptor = _carDescriptors.Find(descriptor => descriptor.CarType == carType);
            CarEntity carEntity = new CarEntity(carDescriptor, pathPositions);
            return carEntity;
        }
    }
}
