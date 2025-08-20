using System;
using System.Collections.Generic;
using Core.ModalUI;
using Core.Services.Updater;
using GameSession.InputReader;
using GameSession.UI.WarehouseInventory.Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSession.Map
{
    public class MapCameraController : IDisposable
    {
        private readonly Camera _cam;
        private readonly float _zoomStep;
        private readonly float _minCamSize;
        private readonly float _maxCamSize;
        private readonly float _mapMinX;
        private readonly float _mapMaxX;
        private readonly float _mapMinY;
        private readonly float _mapMaxY;
        /*private readonly IWarehouseInventoryState _warehouseInventoryState;*/
        private readonly List<ISceneInputSource> _inputSources;
        
        private Vector3 _camDragOrigin;
        private ISceneInputSource _clickOverObjectInputSource;
        private ISceneInputSource _currentClickInputSource;


        public MapCameraController(Camera cam, float zoomStep, float minCamSize, TilemapRenderer mapRenderer, 
            /*IWarehouseInventoryState warehouseInventoryState,*/ List<ISceneInputSource> inputSources)
        {
            _cam = cam;
            _zoomStep = zoomStep;
            _minCamSize = minCamSize;
            
            _maxCamSize = _cam.orthographicSize;
        
            _mapMinX = mapRenderer.bounds.center.x - mapRenderer.bounds.size.x / 2f;
            _mapMaxX = mapRenderer.bounds.center.x + mapRenderer.bounds.size.x / 2f;

            _mapMinY = mapRenderer.bounds.center.y - mapRenderer.bounds.size.y / 2f;
            _mapMaxY = mapRenderer.bounds.center.y + mapRenderer.bounds.size.y / 2f - 1f; //Cutting out trees outside the map with "-1f"

            /*_warehouseInventoryState = warehouseInventoryState;*/
            _inputSources = inputSources;
            
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
            foreach (var inputSource in _inputSources)
            {
                inputSource.ClickDown += TryRegisterClickOverMapObject;
                inputSource.ClickHoldOriginUpdated += UpdateCamDragOrigin;
            }
        }
        
        public Vector3 ConvertToMapPosition(Vector3 screenPosition)
        {
            return _cam.ScreenToWorldPoint(screenPosition);
        }
        
        void IDisposable.Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
            foreach (var inputSource in _inputSources)
            {
                inputSource.ClickDown -= TryRegisterClickOverMapObject;
                inputSource.ClickHoldOriginUpdated -= UpdateCamDragOrigin;
            }
        }
    
        private void OnUpdate()
        {
            if (_clickOverObjectInputSource is { IsClickHeld: true }) return;
            _clickOverObjectInputSource = null;
            
            if (ModalUIController.Instance.IsModalUIShown /*|| _warehouseInventoryState.IsWarehouseInventoryUIShown*/) return;
            DragCamera();
            ZoomCamera();
        }

        private void TryRegisterClickOverMapObject(ISceneInputSource clickInputSource)
        {
            if (_clickOverObjectInputSource != null) return;
            Vector3 clickPosition = ConvertToMapPosition(clickInputSource.PointerPosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
            _clickOverObjectInputSource = hit.collider != null ? clickInputSource : null;
        }

        private void UpdateCamDragOrigin(Vector3 newDragOrigin)
        {
            _camDragOrigin = ConvertToMapPosition(newDragOrigin);
        }
        
        private void DragCamera()
        {
            _currentClickInputSource ??= _inputSources.Find(inputSource => inputSource.IsClickHeld);
            if (_currentClickInputSource == null) return;
            
            if (!_currentClickInputSource.IsClickHeld)
            {
                _currentClickInputSource = null;
            }
            else
            {
                Vector3 dragDelta = _camDragOrigin - ConvertToMapPosition(_currentClickInputSource.PointerPosition);
                _cam.transform.position = ClampCamera(_cam.transform.position + dragDelta);
            }
        }

        private void ZoomCamera()
        {
            var currentZoomInputSource = _currentClickInputSource;
            currentZoomInputSource ??= _inputSources.Find(inputSource => inputSource.ZoomDelta != 0);
            if (currentZoomInputSource == null || currentZoomInputSource.ZoomDelta == 0) return;
            
            Vector3 pointerPositionBeforeZoom = ConvertToMapPosition(currentZoomInputSource.PointerPosition);
            
            float newSize = _cam.orthographicSize;
            newSize -= currentZoomInputSource.ZoomDelta * _zoomStep;
            _cam.orthographicSize = Mathf.Clamp(newSize, _minCamSize, _maxCamSize);
            
            Vector3 pointerPositionAfterZoom = ConvertToMapPosition(currentZoomInputSource.PointerPosition);
            Vector3 cameraShiftDelta = pointerPositionBeforeZoom - pointerPositionAfterZoom;
            
            _cam.transform.position = ClampCamera(_cam.transform.position + cameraShiftDelta);
        }

        private Vector3 ClampCamera(Vector3 targetPosition)
        {
            float camHeight = _cam.orthographicSize;
            float camWidth = _cam.orthographicSize * _cam.aspect;

            float minX = _mapMinX + camWidth;
            float maxX = _mapMaxX - camWidth;
            float minY = _mapMinY + camHeight;
            float maxY = _mapMaxY - camHeight;

            float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
            float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

            return new Vector3(newX, newY, targetPosition.z);
        }
    }
}