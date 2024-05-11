using System;
using System.Collections.Generic;
using System.Linq;
using CarsSystem;
using CarsSystem.Enums;
using Core.Services.Updater;
using Core.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PathBuilding
{
    public class PathDrawer : IDisposable
    {
        private readonly LineRenderer _lineRenderer;
        private readonly CarSystem _carSystem;
        private readonly Camera _camera;
        private readonly List<PathPointDescriptor> _builtPath;
        private readonly PathPointDescriptor[] _pathPoints;
        
        private bool _isDragging;
        private Vector3 _nextPointCoords;
        private int _linePointIndex;
        
        
        public PathDrawer(LineRenderer lineRenderer, CarSystem carSystem)
        {
            _lineRenderer = lineRenderer;
            _carSystem = carSystem;
            _builtPath = new List<PathPointDescriptor>();
            _camera = Camera.main;
            _lineRenderer.positionCount = 0;
            _pathPoints = Object.FindObjectsOfType<PathPointDescriptor>();
            
            StartDrawingPath();
        }

        public void StartDrawingPath()
        {
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
            
            foreach (var point in _pathPoints)
            {
                point.gameObject.SetActive(true);
            }
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
            CheckDragging();
            CheckMouse();
            CheckPathCancellation();
        }
        
        private void CheckDragging()
        {
            if (_isDragging)
            {
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = -1f;
                _lineRenderer.SetPosition(_linePointIndex, mousePosition);
                _nextPointCoords = mousePosition;
            }
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
            if (Input.GetMouseButtonDown(1))
            {
                if (_linePointIndex > 1)
                {
                    _lineRenderer.positionCount -= 1;
                    _linePointIndex -= 1;
                }
                else
                {
                    _lineRenderer.positionCount = 0;
                    _linePointIndex = 0;
                    _isDragging = false;
                }
                
                if (_builtPath.Count > 0)
                {
                    _builtPath.RemoveAt(_builtPath.Count - 1);
                }
            }
        }
        
        private void ChooseFirstPoint()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hasHit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hasHit.collider && hasHit.collider.TryGetComponent(out PathPointDescriptor currentPoint)
                    && currentPoint.IsStartingPoint)
                {
                    _builtPath.Add(currentPoint);
                    _isDragging = true;
                    Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.z = -1f;
                    _lineRenderer.positionCount += 2;
                    _lineRenderer.SetPosition(_linePointIndex, mousePosition);
                    _linePointIndex += 1;
                }
            }
        }

        private void ChooseNextPoint()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hasHit = Physics2D.Raycast(_nextPointCoords, Vector2.zero);
                if (hasHit.collider && hasHit.collider.TryGetComponent(out PathPointDescriptor currentPoint) 
                    && _builtPath.Last().NextPointsIds.Contains(currentPoint.Id) && _builtPath.All(point => point.Id != currentPoint.Id))
                {
                    if (currentPoint.IsEndingPoint)
                    {
                        _isDragging = false;
                        QuestionUIController.Instance.ShowQuestion("Do you want to finish path building?", () =>
                        {
                            _builtPath.Add(currentPoint);
                            
                            Vector3[] pathPositions = new Vector3[_lineRenderer.positionCount];
                            _lineRenderer.GetPositions(pathPositions);
                            _carSystem.SpawnCar(CarType.Pickup, pathPositions);
                        
                            Dispose();
                            TurnPointsOff();
                        }, () =>
                        {
                            _isDragging = true;
                        });
                        
                    }
                    else
                    {
                        _builtPath.Add(currentPoint);
                        _lineRenderer.positionCount += 1;
                        _linePointIndex += 1;
                    }
                }
            }
        }

        private void TurnPointsOff()
        {
            foreach (var point in _pathPoints)
            {
                point.gameObject.SetActive(false);
            }
        }
    }
}