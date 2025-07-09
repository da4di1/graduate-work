using System;

namespace Core.ModalUI.ModalInterfaces.Dialog
{
    public interface IDialogUIDisplayer
    {
        void Show(string text, Action okayButtonClicked);
    }
}