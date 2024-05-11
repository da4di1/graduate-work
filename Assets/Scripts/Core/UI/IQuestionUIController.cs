using System;

namespace Core.UI
{
    public interface IQuestionUIController
    {
        void ShowQuestion(string text, Action yesButtonClicked, Action noButtonClicked);
    }
}