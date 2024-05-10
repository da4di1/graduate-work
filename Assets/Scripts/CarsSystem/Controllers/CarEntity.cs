using System;
using CarsSystem.Behaviour;
using CarsSystem.Data;
using Core.Services.Updater;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CarsSystem.Controllers
{
    public class CarEntity : IDisposable
    {
        private readonly Vector3[] _pathPositions;
        
        private SceneCar _carBehaviour;
        private Vector2 _nextPosition;
        private int _movementPointIndex;
        private bool _isMovingBack;
        
        public CarDescriptor Descriptor { get; private set; }


        public CarEntity(CarDescriptor descriptor, Vector3[] pathPositions)
        {
            Descriptor = descriptor;
            _pathPositions = pathPositions;
        }

        public void Initialize(SceneCar carBehaviour)
        {
            _carBehaviour = carBehaviour;

            ProjectUpdater.Instance.FixedUpdateCalled += OnFixedUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.FixedUpdateCalled -= OnFixedUpdate;
        }

        private void OnFixedUpdate()
        {
            _nextPosition = _pathPositions[_movementPointIndex];
            _carBehaviour.Move(_nextPosition, Descriptor.Speed * Time.deltaTime);
            float distance = Vector2.Distance(_carBehaviour.Position, _nextPosition);
            
            if (distance <= Mathf.Epsilon)
            {
                if (!_isMovingBack)
                {
                    _movementPointIndex++;
                    if (_movementPointIndex > _pathPositions.Length - 1)
                    {
                        _isMovingBack = true;
                        _movementPointIndex--;
                    }
                }
                else
                {
                    _movementPointIndex--;
                    if (_movementPointIndex < 0)
                    {
                        Dispose();
                        Object.Destroy(_carBehaviour.gameObject);
                    }
                }
            }
        }
    }
}
