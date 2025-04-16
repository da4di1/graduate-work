using System;
using Core.Services.Updater;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public class DialogUIDisplayer : MonoBehaviour, IDialogUIDisplayer, IModalUI
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _okayButton;

        private string _cachedText;
        private Action _cachedOkayButtonClicked;
        
        public bool IsShown => gameObject.activeSelf;
        
        public event Action Appeared;
        public event Action Disappeared;
        

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Show(string text, Action okayButtonClicked)
        {
            if (ProjectUpdater.Instance != null && !ProjectUpdater.Instance.IsPaused)
            {
                _cachedText = text;
                _cachedOkayButtonClicked = okayButtonClicked;
            }
            
            Appeared?.Invoke();
            gameObject.SetActive(true);

            _text.text = text;
            _okayButton.onClick.AddListener(() =>
            {
                Hide();
                okayButtonClicked?.Invoke();
            });
        }

        public void Reshow()
        {
            Show(_cachedText, _cachedOkayButtonClicked);
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
            _okayButton.onClick.RemoveAllListeners();
        }
    }
}