using System;
using System.Collections.Generic;
using System.Linq;
using Core.UI.ModalUI.ModalInterfaces;
using UnityEngine;

namespace Core.UI.ModalUI
{
    public class ModalUIController : MonoBehaviour
    {
        public static ModalUIController Instance { get; private set; }

        [SerializeField] private DialogUIController _dialog;
        [SerializeField] private QuestionUIController _question;

        private List<IModalUI> _modalInterfaces;

        public IDialogUIController Dialog => _dialog;
        public IQuestionUIController Question => _question;
        public bool IsModalUIShown { get; private set; }

        public event Action ModalUIAppeared;
        public event Action ModalUIDisappeared;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _modalInterfaces = new List<IModalUI>
            {
                _dialog,
                _question,
            };
        }

        private void Start()
        {
            foreach (var modalUI in _modalInterfaces)
            {
                modalUI.Appeared += OnModalUIAppeared;
                modalUI.Disappeared += OnModalUIDisappeared;
            }
        }

        private void OnDestroy()
        {
            foreach (var modalUI in _modalInterfaces)
            {
                modalUI.Appeared -= OnModalUIAppeared;
                modalUI.Disappeared -= OnModalUIDisappeared;
            }
        }

        public void HideModalInterfaces()
        {
            foreach (var modalUI in _modalInterfaces)
            {
                modalUI.Hide();
            }
        }

        private void OnModalUIAppeared()
        {
            if (IsModalUIShown) return;
            IsModalUIShown = true;
            ModalUIAppeared?.Invoke();
        }

        private void OnModalUIDisappeared()
        {
            if (_modalInterfaces.Any(modalUI => modalUI.IsShown)) return;
            IsModalUIShown = false;
            ModalUIDisappeared?.Invoke();
        }
    }
}