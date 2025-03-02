using System;

namespace Core.UI.ModalUI.QuestionUI
{
    public interface IQuestionUIController
    {
        bool IsQuestionUIShown { get; }
        event Action QuestionAppeared;
        event Action QuestionDisappeared;
        void ShowQuestion(string text, Action yesButtonClicked, Action noButtonClicked);
    }
}