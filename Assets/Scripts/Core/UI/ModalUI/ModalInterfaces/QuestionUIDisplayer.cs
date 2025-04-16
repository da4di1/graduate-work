using System;
using Core.Services.Updater;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public class QuestionUIDisplayer : MonoBehaviour, IQuestionUIDisplayer, IModalUI
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private string _cachedText;
        private Action _cachedYesButtonClicked;
        private Action _cachedNoButtonClicked;
        
        public bool IsShown => gameObject.activeSelf;
        
        public event Action Appeared;
        public event Action Disappeared;
        

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Show(string text, Action yesButtonClicked, Action noButtonClicked)
        {
            if (ProjectUpdater.Instance != null && !ProjectUpdater.Instance.IsPaused)
            {
                _cachedText = text;
                _cachedYesButtonClicked = yesButtonClicked;
                _cachedNoButtonClicked = noButtonClicked;
            }
            
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
        
        public void Reshow()
        {
            Show(_cachedText, _cachedYesButtonClicked, _cachedNoButtonClicked);
        }
        
        public bool Hide()
        {
            if (!IsShown) return false;
            gameObject.SetActive(false);
            RemoveButtonsListeners();
            Disappeared?.Invoke();
            return true;
        }

        private void RemoveButtonsListeners()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}