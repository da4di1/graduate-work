using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI;
using Core.Services.Updater;
using GameSession.Cars;
using GameSession.Cars.Enums;
using UnityEngine;

namespace GameSession.PathBuilding
{
    public class PathDrawer : IDisposable
    {
        private readonly LineRenderer _lineRenderer;
        private readonly Transform _pathPoints;
        private readonly CarsSystem _carsSystem;
        private readonly Camera _camera;
        private readonly List<PathPointDescriptor> _builtPath;
        
        private Vector3 _mousePosition;
        private int _linePointIndex;
        private bool _isDragging;
        
        
        public PathDrawer(LineRenderer lineRenderer, Transform pathPoints, CarsSystem carsSystem)
        {
            _lineRenderer = lineRenderer;
            _pathPoints = pathPoints;
            _carsSystem = carsSystem;
            _builtPath = new List<PathPointDescriptor>();
            _camera = Camera.main;
            _lineRenderer.positionCount = 0;
            TurnPointsOff();
        }

        public void StartDrawingPath()
        {
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
            
            _pathPoints.gameObject.SetActive(true);
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
            
            _lineRenderer.positionCount = 0;
            _linePointIndex = 0;
            _builtPath.Clear();
        }

        private void OnUpdate()
        {
            if (ModalUIController.Instance.IsModalUIShown) return;
            _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition.z = -1f; //for line renderer to be visible
            CheckDragging();
            CheckMouse();
            CheckPathCancellation();
        }
        
        private void CheckDragging()
        {
            if (!_isDragging) return;
            _lineRenderer.SetPosition(_linePointIndex, _mousePosition);
        }
        
        private void CheckMouse()
        {
            if (!_isDragging)
            {
                ChooseFirstPoint();
            }
            else
            {
                ChooseNextPoint();
            }
        }

        private void CheckPathCancellation()
        {
            if (!Input.GetMouseButtonDown(1)) return;
            if (_linePointIndex > 1)
            {
                _lineRenderer.positionCount -= 1;
                _linePointIndex -= 1;
                _builtPath.RemoveAt(_builtPath.Count - 1);
            }
            else
            {
                _lineRenderer.positionCount = 0;
                _linePointIndex = 0;
                _builtPath.Clear();
                _isDragging = false;
            }
        }
        
        private void ChooseFirstPoint()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            RaycastHit2D hasHit = Physics2D.Raycast(_mousePosition, Vector2.zero);
            if (!hasHit.collider || !hasHit.collider.TryGetComponent(out PathPointDescriptor currentPoint) || !currentPoint.IsStartingPoint) return;
            _builtPath.Add(currentPoint);
            _isDragging = true;
            _lineRenderer.positionCount += 2;
            _lineRenderer.SetPosition(_linePointIndex, _mousePosition);
            _linePointIndex += 1;
        }

        private void ChooseNextPoint()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            RaycastHit2D hasHit = Physics2D.Raycast(_mousePosition, Vector2.zero);
            if (!hasHit.collider || !hasHit.collider.TryGetComponent(out PathPointDescriptor currentPoint)
                                 || !_builtPath.Last().NextPointsIds.Contains(currentPoint.Id) 
                                 || _builtPath.Any(point => point.Id == currentPoint.Id)) return;
            if (currentPoint.IsEndingPoint)
            {
                ModalUIController.Instance.Question.Show("Do you want to finish path building?", () =>
                {
                    _builtPath.Add(currentPoint);
                            
                    Vector3[] pathPositions = new Vector3[_lineRenderer.positionCount];
                    _lineRenderer.GetPositions(pathPositions);
                    _carsSystem.StartCar(CarType.Pickup, pathPositions);

                    _isDragging = false;
                    Dispose();
                    TurnPointsOff();
                }, null);
            }
            else
            {
                _builtPath.Add(currentPoint);
                _lineRenderer.positionCount += 1;
                _linePointIndex += 1;
            }
        }

        private void TurnPointsOff()
        {
            _pathPoints.gameObject.SetActive(false);
        }
    }
}