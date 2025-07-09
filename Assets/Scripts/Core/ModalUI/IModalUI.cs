using System;

namespace Core.ModalUI
{
    public interface IModalUI
    {
        bool IsShown { get; }
        event Action Appeared;
        event Action Disappeared;
        void Reshow();
        bool Hide();
    }
}