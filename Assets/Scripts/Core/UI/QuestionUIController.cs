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
            gameObject.SetActive(true);

            _text.text = text;
            _yesButton.onClick.AddListener(() =>
            {
                Hide();
                yesButtonClicked();
                RemoveButtonListeners();
            });
            _noButton.onClick.AddListener(() =>
            {
                Hide();
                noButtonClicked();
                RemoveButtonListeners();
            });
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void RemoveButtonListeners()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}