using System;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public interface IDialogUIDisplayer
    {
        void Show(string text, Action okayButtonClicked);
    }
}