using System;

namespace Core.UI.DialogUI
{
    public interface IDialogUIController
    {
        bool IsDialogUIShown { get; }
        event Action DialogAppeared;
        event Action DialogDisappeared;
        void ShowDialog(string text, Action okayButtonClicked);
    }
}