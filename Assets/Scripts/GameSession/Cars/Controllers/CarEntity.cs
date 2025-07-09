using System;
using Core.Services.Updater;
using GameSession.Cars.Behaviour;
using GameSession.Cars.Data;
using GameSession.Cars.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameSession.Cars.Controllers
{
    public class CarEntity : IDisposable, ICarInformation
    {
        private readonly Vector3[] _pathPositions;
        
        private SceneCar _carBehaviour;
        private Vector2 _nextPosition;
        private int _movementPointIndex;
        private bool _isMovingBack;
        
        public CarDescriptor Descriptor { get; }


        public CarEntity(CarDescriptor descriptor, Vector3[] pathPositions)
        {
            Descriptor = descriptor;
            _pathPositions = pathPositions;
        }

        public void Initialize(SceneCar carBehaviour)
        {
            _carBehaviour = carBehaviour;

            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }

        private void OnUpdate()
        {
            _nextPosition = _pathPositions[_movementPointIndex];
            _carBehaviour.Move(_nextPosition, Descriptor.Speed);
            float distance = Vector2.Distance(_carBehaviour.Position, _nextPosition);

            if (!(distance <= Mathf.Epsilon)) return;
            if (!_isMovingBack)
            {
                _movementPointIndex++;
                if (_movementPointIndex <= _pathPositions.Length - 1) return;
                _isMovingBack = true;
                _movementPointIndex--;
            }
            else
            {
                _movementPointIndex--;
                if (_movementPointIndex >= 0) return;
                Dispose();
                Object.Destroy(_carBehaviour.gameObject);
            }
        }
    }
}