using System;
using System.Collections.Generic;
using CarsSystem.Behaviour;
using CarsSystem.Controllers;
using CarsSystem.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CarsSystem
{
    public class CarSystem : IDisposable
    {
        private readonly CarsFactory _carsFactory;
        private readonly SceneCar _sceneCar;
        private readonly Transform _transform;
        private readonly List<IDisposable> _disposables;


        public CarSystem(CarsFactory carsFactory)
        {
            _disposables = new List<IDisposable>();
            _carsFactory = carsFactory;
            _sceneCar = Resources.Load<SceneCar>($"{nameof(CarsSystem)}/{nameof(SceneCar)}");
            GameObject gameObject = new GameObject
            {
                name = nameof(CarsSystem)
            };
            _transform = gameObject.transform;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        public void SpawnCar(CarType carType, Vector3[] pathPositions) =>
            SpawnCar(_carsFactory.CreateCar(carType, pathPositions), pathPositions[0]);

        private void SpawnCar(CarEntity carEntity, Vector2 position)
        {
            SceneCar sceneCar = Object.Instantiate(_sceneCar, _transform);
            sceneCar.SetCar(carEntity.Descriptor.VerticalSprite, position);
            carEntity.Initialize(sceneCar);
            _disposables.Add(carEntity);
        }
    }
}