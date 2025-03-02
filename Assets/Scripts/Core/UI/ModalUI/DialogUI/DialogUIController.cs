using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.ModalUI.DialogUI
{
    public class DialogUIController : MonoBehaviour, IModalUI
    {
        /*public static IDialogUIController Instance { get; private set; }*/

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _okayButton;
        
        /*public bool IsDialogUIShown { get; private set; }*/

        public event Action Appeared;
        public event Action Disappeared;

        
        /*private void Awake()
        {
            /*Instance = this;#1#
            
            Hide();
        }*/

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Show(string text, Action okayButtonClicked)
        {
            if (ModalUIController.Instance.IsModalUIShown) return;
            
            Appeared?.Invoke();
            gameObject.SetActive(true);
            /*IsDialogUIShown = true;*/

            _text.text = text;
            _okayButton.onClick.AddListener(() =>
            {
                Hide();
                okayButtonClicked?.Invoke();
                /*RemoveButtonsListeners();*/
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            RemoveButtonsListeners();
            if (!ModalUIController.Instance.IsModalUIShown) return;
            
            /*IsDialogUIShown = false;*/
            Disappeared?.Invoke();
        }

        private void RemoveButtonsListeners()
        {
            _okayButton.onClick.RemoveAllListeners();
        }
    }
}