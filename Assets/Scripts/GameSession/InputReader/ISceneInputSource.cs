using System;
using UnityEngine;

namespace GameSession.InputReader
{
    public interface ISceneInputSource
    {
        float ZoomDelta { get; }
        Vector3 PointerPosition { get; }
        bool IsClickHeld { get; }
        
        event Action<ISceneInputSource> ClickDown;
        event Action<ISceneInputSource> ClickUp;
        event Action<Vector3> ClickHoldOriginUpdated;
    }
}