using System;

namespace Core.ModalUI.ModalInterfaces.Question
{
    public interface IQuestionUIDisplayer
    {
        void Show(string text, Action yesButtonClicked, Action noButtonClicked);
    }
}