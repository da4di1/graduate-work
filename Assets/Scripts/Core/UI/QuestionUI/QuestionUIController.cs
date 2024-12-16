using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.QuestionUI
{
    public class QuestionUIController : MonoBehaviour, IQuestionUIController
    {
        public static IQuestionUIController Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        public bool IsQuestionUIShown { get; private set; }

        public event Action QuestionAppeared;
        public event Action QuestionDisappeared;

        
        private void Awake()
        {
            Instance = this;
            
            Hide();
        }

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void ShowQuestion(string text, Action yesButtonClicked, Action noButtonClicked)
        {
            QuestionAppeared?.Invoke();
            gameObject.SetActive(true);
            IsQuestionUIShown = true;

            _text.text = text;
            _yesButton.onClick.AddListener(() =>
            {
                Hide();
                yesButtonClicked?.Invoke();
                RemoveButtonsListeners();
            });
            _noButton.onClick.AddListener(() =>
            {
                Hide();
                noButtonClicked?.Invoke();
                RemoveButtonsListeners();
            });
        }

        private void Hide()
        {
            IsQuestionUIShown = false;
            QuestionDisappeared?.Invoke();
            gameObject.SetActive(false);
        }

        private void RemoveButtonsListeners()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}