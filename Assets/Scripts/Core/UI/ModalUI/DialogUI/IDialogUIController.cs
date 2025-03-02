using System;

namespace Core.UI.ModalUI.DialogUI
{
    public interface IDialogUIController
    {
        /*bool IsDialogUIShown { get; }
        event Action DialogAppeared;
        event Action DialogDisappeared;*/
        void ShowDialog(string text, Action okayButtonClicked);
    }
}