using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private float _zoomStep;
        [SerializeField] private float _minCamSize;
        [SerializeField] private TilemapRenderer _mapRenderer;

        private float _maxCamSize;
        private float _mapMinX;
        private float _mapMaxX;
        private float _mapMinY;
        private float _mapMaxY;
        private Vector3 _dragOrigin;


        private void Awake()
        {
            _maxCamSize = _cam.orthographicSize;
        
            _mapMinX = _mapRenderer.bounds.center.x - _mapRenderer.bounds.size.x / 2f;
            _mapMaxX = _mapRenderer.bounds.center.x + _mapRenderer.bounds.size.x / 2f;

            _mapMinY = _mapRenderer.bounds.center.y - _mapRenderer.bounds.size.y / 2f;
            _mapMaxY = _mapRenderer.bounds.center.y + _mapRenderer.bounds.size.y / 2f - 1f;
        }
    
        private void Update()
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
    }
}