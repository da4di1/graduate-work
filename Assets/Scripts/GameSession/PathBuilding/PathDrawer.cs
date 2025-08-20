using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI;
using Core.Services.Updater;
using GameSession.Cars;
using GameSession.Cars.Enums;
using GameSession.InputReader;
using GameSession.Map;
using GameSession.Warehouses.Enums;
using UnityEngine;

namespace GameSession.PathBuilding
{
    public class PathDrawer : IDisposable
    {
        private readonly LineRenderer _lineRenderer;
        private readonly MapCameraController _mapController;
        private readonly CarsSystem _carsSystem;
        private readonly List<PathPointDescriptor> _pathPoints;
        private readonly List<ISceneInputSource> _inputSources;
        private readonly List<PathPointDescriptor> _builtPath;
        
        private ISceneInputSource _currentClickInputSource;
        private PathPointDescriptor _clickedPathPoint;
        private bool _isClickInProgress;
        
        
        public PathDrawer(List<PathPointDescriptor> pathPoints, LineRenderer lineRenderer, MapCameraController mapController, CarsSystem carsSystem,
            List<ISceneInputSource> inputSources)
        {
            _pathPoints = pathPoints;
            _lineRenderer = lineRenderer;
            _mapController = mapController;
            _carsSystem = carsSystem;
            _inputSources = inputSources;
            _builtPath = new List<PathPointDescriptor>();
            _lineRenderer.positionCount = 0;
        }

        public void StartDrawingPathFrom(WarehouseID warehouseID)
        {
            PathPointDescriptor startingPathPoint = _pathPoints.Find(pathPoint => pathPoint.AdjacentWarehouseID == warehouseID);
            if (!startingPathPoint) return;
            
            foreach (var pathPoint in _pathPoints)
            {
                if (pathPoint.AdjacentWarehouseID != WarehouseID.None || pathPoint.AdjacentBusinessID)
                {
                    pathPoint.gameObject.SetActive(true);
                }
            }
            
            _builtPath.Add(startingPathPoint);
            startingPathPoint.SetNeighborPointsActive(true);
            AddPointToLine(startingPathPoint);
            
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
            foreach (var inputSource in _inputSources)
            {
                inputSource.ClickDown += TryStartClickOnPathPoint;
                inputSource.ClickUp += HandleClickRelease;
            }
        }

        public void Dispose()
        {
            foreach (var pathPoint in _pathPoints)
            {
                if (pathPoint&& pathPoint.gameObject)
                {
                    pathPoint.gameObject.SetActive(false);
                }
            }
            
            _builtPath.Clear();
            _lineRenderer.positionCount = 0;
            
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
            foreach (var inputSource in _inputSources)
            {
                inputSource.ClickDown -= TryStartClickOnPathPoint;
                inputSource.ClickUp -= HandleClickRelease;
            }
        }

        private void OnUpdate()
        {
            if (ModalUIController.Instance.IsModalUIShown) return;
            ValidateOngoingClick();
        }

        private void AddPointToLine(PathPointDescriptor pathPoint)
        {
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, pathPoint.transform.position);
        }

        private void TryStartClickOnPathPoint(ISceneInputSource clickInputSource)
        {
            if (_currentClickInputSource != null) return;
            
            Vector3 mousePosition = _mapController.ConvertToMapPosition(clickInputSource.PointerPosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider == null || !hit.collider.TryGetComponent(out PathPointDescriptor pathPoint)) return;
            
            _currentClickInputSource = clickInputSource;
            _clickedPathPoint = pathPoint;
            _isClickInProgress = true;
        }
        
        private void HandleClickRelease(ISceneInputSource clickInputSource)
        {
            if (_currentClickInputSource != clickInputSource) return;
            if (_isClickInProgress)
            {
                TryChangePath();
            }
            
            _currentClickInputSource = null;
            _clickedPathPoint = null;
            _isClickInProgress = false;
        }

        private void ValidateOngoingClick()
        {
            if (!_isClickInProgress || _currentClickInputSource is not { IsClickHeld: true } || _clickedPathPoint == null) return;
            
            Vector3 mousePosition = _mapController.ConvertToMapPosition(_currentClickInputSource.PointerPosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider == null || hit.collider != _clickedPathPoint.GetComponent<Collider2D>()) return;
            
            _isClickInProgress = false;
        }

        private void TryChangePath()
        {
            if (_builtPath.Last() == _clickedPathPoint)
            {
                RemoveClickedPointFromPath();
            }
            else if (_builtPath.Last().NeighborPoints.Contains(_clickedPathPoint) && _builtPath.All(pathPoint => pathPoint != _clickedPathPoint))
            {
                AddClickedPointToPath();
            }
        }

        private void RemoveClickedPointFromPath()
        {
            if (_lineRenderer.positionCount > 1)
            {
                _clickedPathPoint.SetNeighborPointsActive(false, _builtPath);
                _builtPath.Remove(_clickedPathPoint);
                _builtPath.Last().SetNeighborPointsActive(true, _builtPath);
                _lineRenderer.positionCount -= 1;
            }
            else
            {
                ModalUIController.Instance.Question.Show("Do you want to finish path building?", Dispose, null);
            }
        }

        private void AddClickedPointToPath()
        {
            if (_clickedPathPoint.AdjacentWarehouseID != WarehouseID.None || _clickedPathPoint.AdjacentBusinessID)
            {
                ModalUIController.Instance.Question.Show("Do you want to finish path building?", () =>
                {
                    ChangeLastPathPoint();
                    AddPointToLine(_clickedPathPoint);
                            
                    Vector3[] pathPositions = new Vector3[_lineRenderer.positionCount];
                    _lineRenderer.GetPositions(pathPositions);
                    _carsSystem.StartCar(CarType.Pickup, pathPositions);
                    
                    Dispose();
                }, null);
            }
            else
            {
                ChangeLastPathPoint();
                _clickedPathPoint.SetNeighborPointsActive(true, _builtPath);
                AddPointToLine(_clickedPathPoint);
            }
        }

        private void ChangeLastPathPoint()
        {
            PathPointDescriptor previousLastPathPoint = _builtPath.Last();
            _builtPath.Add(_clickedPathPoint);
            previousLastPathPoint.SetNeighborPointsActive(false, _builtPath);
        }
    }
}