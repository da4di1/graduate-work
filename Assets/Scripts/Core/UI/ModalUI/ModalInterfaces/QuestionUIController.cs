using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public class QuestionUIController : MonoBehaviour, IQuestionUIController, IModalUI
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        public bool IsShown => gameObject.activeSelf;
        
        public event Action Appeared;
        public event Action Disappeared;
        

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Show(string text, Action yesButtonClicked, Action noButtonClicked)
        {
            Appeared?.Invoke();
            gameObject.SetActive(true);

            _text.text = text;
            _yesButton.onClick.AddListener(() =>
            {
                Hide();
                yesButtonClicked?.Invoke();
            });
            _noButton.onClick.AddListener(() =>
            {
                Hide();
                noButtonClicked?.Invoke();
            });
        }

        public void Hide()
        {
            if (!IsShown) return;
            gameObject.SetActive(false);
            RemoveButtonsListeners();
            Disappeared?.Invoke();
        }

        private void RemoveButtonsListeners()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}