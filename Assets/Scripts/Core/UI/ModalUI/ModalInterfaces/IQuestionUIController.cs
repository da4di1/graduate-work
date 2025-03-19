using System;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public interface IQuestionUIController
    {
        void Show(string text, Action yesButtonClicked, Action noButtonClicked);
    }
}