using System;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public interface IDialogUIController
    {
        void Show(string text, Action okayButtonClicked);
    }
}