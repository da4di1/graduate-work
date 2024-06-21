using System;

namespace Core.UI
{
    public interface IQuestionUIController
    {
        event Action QuestionAppeared;
        event Action QuestionDisappeared;
        void ShowQuestion(string text, Action yesButtonClicked, Action noButtonClicked);
        bool IsQuestionUIShown { get; }
    }
}