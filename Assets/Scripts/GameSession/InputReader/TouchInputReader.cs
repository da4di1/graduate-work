using System;
using Core.Services.Updater;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSession.InputReader
{
    public class TouchInputReader : ISceneInputSource, IDisposable
    {
        private int _previousTouchCount;
        
        public float ZoomDelta { get; private set; }
        public Vector3 PointerPosition { get; private set; }
        public bool IsClickHeld { get; private set; }
        
        public event Action<ISceneInputSource> ClickDown;
        public event Action<ISceneInputSource> ClickUp;
        public event Action<Vector3> ClickHoldOriginUpdated;


        public TouchInputReader()
        {
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }
        
        private void OnUpdate()
        {
            bool isPointerOverUI = IsAnyPointerOverUI();
            int touchCount = Input.touchCount;
            ZoomDelta = 0;
            
            if (touchCount == 0 || AreAllPointersOverUI())
            {
                IsClickHeld = false;
                _previousTouchCount = 0;
                return;
            }
            
            Touch firstTouch = Input.GetTouch(0);

            switch (touchCount)
            {
                case 1:
                    HandleSingleTouch(firstTouch, isPointerOverUI);
                    break;
                case >= 2 when !isPointerOverUI:
                    HandleMultiTouch(firstTouch, Input.GetTouch(1));
                    break;
            }

            if (!isPointerOverUI)
            {
                CheckIfClickHeld(firstTouch, touchCount);
            }
            
            _previousTouchCount = touchCount;
        }

        private bool IsAnyPointerOverUI()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    return true; 
                }
            }
            return false;
        }
        
        private bool AreAllPointersOverUI()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    return false;
                }
            }
            return true;
        }

        private void HandleSingleTouch(Touch touch, bool isPointerOverUI)
        {
            PointerPosition = touch.position;
            switch (touch.phase)
            {
                case TouchPhase.Began when !isPointerOverUI:
                    ClickDown?.Invoke(this);
                    break;
                case TouchPhase.Ended: 
                case TouchPhase.Canceled:
                    ClickUp?.Invoke(this);
                    break;
            }
        }

        private void HandleMultiTouch(Touch firstTouch, Touch secondTouch)
        {
            PointerPosition = (firstTouch.position + secondTouch.position) / 2f;
                    
            Vector2 previousFirstTouch = firstTouch.position - firstTouch.deltaPosition;
            Vector2 previousSecondTouch = secondTouch.position - secondTouch.deltaPosition;
                    
            float previousTouchesDistance = Vector2.Distance(previousFirstTouch, previousSecondTouch);
            float currentTouchesDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
                    
            ZoomDelta = currentTouchesDistance - previousTouchesDistance;
        }

        private void CheckIfClickHeld(Touch touch, int touchCount)
        {
            if (touch.phase is not (TouchPhase.Stationary or TouchPhase.Moved) || (IsClickHeld && _previousTouchCount == touchCount)) return;
            ClickHoldOriginUpdated?.Invoke(PointerPosition);
            IsClickHeld = true;
        }
    }
}