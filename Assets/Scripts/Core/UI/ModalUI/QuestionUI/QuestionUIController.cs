using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.ModalUI.QuestionUI
{
    public class QuestionUIController : MonoBehaviour, IModalUI
    {
        /*public static IQuestionUIController Instance { get; private set; }*/

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        /*public bool IsQuestionUIShown { get; private set; }*/

        public event Action Appeared;
        public event Action Disappeared;

        
        /*private void Awake()
        {
            /*Instance = this;
            #1#
            
            Hide();
        }*/

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Show(string text, Action yesButtonClicked, Action noButtonClicked)
        {
            if (ModalUIController.Instance.IsModalUIShown) return;
            
            Appeared?.Invoke();
            gameObject.SetActive(true);
            /*IsQuestionUIShown = true;*/

            _text.text = text;
            _yesButton.onClick.AddListener(() =>
            {
                Hide();
                yesButtonClicked?.Invoke();
                /*RemoveButtonsListeners();*/
            });
            _noButton.onClick.AddListener(() =>
            {
                Hide();
                noButtonClicked?.Invoke();
                /*RemoveButtonsListeners();*/
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            RemoveButtonsListeners();
            if (!ModalUIController.Instance.IsModalUIShown) return;
            
            /*IsQuestionUIShown = false;*/
            Disappeared?.Invoke();
        }

        private void RemoveButtonsListeners()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}