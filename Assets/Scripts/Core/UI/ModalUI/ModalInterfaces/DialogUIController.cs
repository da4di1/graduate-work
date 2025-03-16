using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.ModalUI.ModalInterfaces
{
    public class DialogUIController : MonoBehaviour, IModalUI
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _okayButton;

        public event Action Appeared;
        public event Action Disappeared;
        

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Show(string text, Action okayButtonClicked)
        {
            if (ModalUIController.Instance.IsModalUIShown) return;
            
            Appeared?.Invoke();
            gameObject.SetActive(true);

            _text.text = text;
            _okayButton.onClick.AddListener(() =>
            {
                Hide();
                okayButtonClicked?.Invoke();
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            RemoveButtonsListeners();
            
            if (!ModalUIController.Instance.IsModalUIShown) return;
            Disappeared?.Invoke();
        }

        private void RemoveButtonsListeners()
        {
            _okayButton.onClick.RemoveAllListeners();
        }
    }
}