using System;
using Core.Services.Updater;
using Core.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class MapCameraController : IDisposable
    {
        private readonly Camera _cam;
        private readonly float _zoomStep;
        private readonly float _minCamSize;
        private readonly TilemapRenderer _mapRenderer;
        private readonly float _maxCamSize;
        private readonly float _mapMinX;
        private readonly float _mapMaxX;
        private readonly float _mapMinY;
        private readonly float _mapMaxY;
        private Vector3 _dragOrigin;
        private bool _isCameraStopped;


        public MapCameraController(Camera cam, float zoomStep, float minCamSize, TilemapRenderer mapRenderer)
        {
            _cam = cam;
            _zoomStep = zoomStep;
            _minCamSize = minCamSize;
            
            _maxCamSize = _cam.orthographicSize;
        
            _mapMinX = mapRenderer.bounds.center.x - mapRenderer.bounds.size.x / 2f;
            _mapMaxX = mapRenderer.bounds.center.x + mapRenderer.bounds.size.x / 2f;

            _mapMinY = mapRenderer.bounds.center.y - mapRenderer.bounds.size.y / 2f;
            _mapMaxY = mapRenderer.bounds.center.y + mapRenderer.bounds.size.y / 2f - 1f;

            QuestionUIController.Instance.QuestionAppeared += StopCamera;
            QuestionUIController.Instance.QuestionDisappeared += StartCameraMovement;
            StartCameraMovement();
        }

        public void Dispose()
        {
            QuestionUIController.Instance.QuestionAppeared -= StopCamera;
            QuestionUIController.Instance.QuestionDisappeared -= StartCameraMovement;
            StopCamera();
        }
    
        private void OnUpdate()
        {
            PanCamera();
            Zoom();
        }
        
        private void PanCamera()
        {
            if (Input.GetMouseButtonDown(0))
                _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButton(0))
            {
                Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);

                _cam.transform.position = ClampCamera(_cam.transform.position + difference); 
            }
        }

        private void Zoom()
        {
            Vector2 scrollDelta = Input.mouseScrollDelta;
            float newSize = _cam.orthographicSize;
        
            if (scrollDelta.y > 0f)
            {
                newSize = _cam.orthographicSize - _zoomStep;
            }
            else if (scrollDelta.y < 0f)
            {
                newSize = _cam.orthographicSize + _zoomStep;
            }
        
            _cam.orthographicSize = Mathf.Clamp(newSize, _minCamSize, _maxCamSize);
            _cam.transform.position = ClampCamera(_cam.transform.position);
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

        private void StartCameraMovement()
        {
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        private void StopCamera()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }
    }
}