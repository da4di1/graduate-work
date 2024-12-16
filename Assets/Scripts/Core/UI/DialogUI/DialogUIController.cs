using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.DialogUI
{
    public class DialogUIController : MonoBehaviour, IDialogUIController
    {
        public static IDialogUIController Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _okayButton;
        
        public bool IsDialogUIShown { get; private set; }

        public event Action DialogAppeared;
        public event Action DialogDisappeared;

        
        private void Awake()
        {
            Instance = this;
            
            Hide();
        }

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void ShowDialog(string text, Action okayButtonClicked)
        {
            DialogAppeared?.Invoke();
            gameObject.SetActive(true);
            IsDialogUIShown = true;

            _text.text = text;
            _okayButton.onClick.AddListener(() =>
            {
                Hide();
                okayButtonClicked?.Invoke();
                RemoveButtonsListeners();
            });
        }

        private void Hide()
        {
            IsDialogUIShown = false;
            DialogDisappeared?.Invoke();
            gameObject.SetActive(false);
        }

        private void RemoveButtonsListeners()
        {
            _okayButton.onClick.RemoveAllListeners();
        }
    }
}