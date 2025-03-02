using System;

namespace Core.UI.ModalUI
{
    public interface IModalUI
    {
        event Action Appeared;
        event Action Disappeared;
        void Hide();
    }
}