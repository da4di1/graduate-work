using System;

namespace Core.Services.Updater
{
    public interface IProjectUpdater
    {
        bool IsPaused { get; }
        event Action UpdateCalled;
    }
}