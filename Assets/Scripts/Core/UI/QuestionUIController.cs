using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
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
            RemoveButtonListeners();
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
                RemoveButtonListeners();
            });
            _noButton.onClick.AddListener(() =>
            {
                Hide();
                noButtonClicked?.Invoke();
                RemoveButtonListeners();
            });
        }

        private void Hide()
        {
            QuestionDisappeared?.Invoke();
            gameObject.SetActive(false);
            IsQuestionUIShown = false;
        }

        private void RemoveButtonListeners()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}