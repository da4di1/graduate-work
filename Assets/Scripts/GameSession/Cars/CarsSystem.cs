using System;
using System.Collections.Generic;
using GameSession.Cars.Behaviour;
using GameSession.Cars.Controllers;
using GameSession.Cars.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameSession.Cars
{
    public class CarsSystem : IDisposable
    {
        private readonly CarsFactory _carsFactory;
        private readonly SceneCar _sceneCar;
        private readonly Transform _transform;
        private readonly List<IDisposable> _disposables;


        public CarsSystem(CarsFactory carsFactory)
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

        public void StartCar(CarType carType, Vector3[] pathPositions) =>
            SpawnCar(_carsFactory.CreateCar(carType, pathPositions), pathPositions[0]);

        private void SpawnCar(CarEntity carEntity, Vector2 position)
        {
            SceneCar sceneCar = Object.Instantiate(_sceneCar, _transform);
            sceneCar.SetCar(carEntity.Descriptor.VerticalSprite, carEntity.Descriptor.HorizontalSprite, position);
            carEntity.Initialize(sceneCar);
            _disposables.Add(carEntity);
        }
    }
}