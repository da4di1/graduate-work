using System;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public interface IQuestionUIDisplayer
    {
        void Show(string text, Action yesButtonClicked, Action noButtonClicked);
    }
}