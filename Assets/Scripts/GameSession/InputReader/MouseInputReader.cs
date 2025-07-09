using System;
using Core.Services.Updater;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSession.InputReader
{
    public class MouseInputReader : ISceneInputSource, IDisposable
    {
        public float ZoomDelta => Input.mouseScrollDelta.y;
        public Vector3 PointerPosition => Input.mousePosition;
        public bool IsClickHeld { get; private set; }
        
        public event Action<ISceneInputSource> ClickDown;
        public event Action<ISceneInputSource> ClickUp;
        public event Action<Vector3> ClickHoldOriginUpdated;


        public MouseInputReader()
        {
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }
        
        private void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            {
                ClickDown?.Invoke(this);
                ClickHoldOriginUpdated?.Invoke(PointerPosition);
                IsClickHeld = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                ClickUp?.Invoke(this);
                IsClickHeld = false;
            }
        }
        
        private bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    }
}