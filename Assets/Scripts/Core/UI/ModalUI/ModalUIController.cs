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
        private List<IModalUI> _modalInterfacesToReshow;

        public IDialogUIController Dialog => _dialog;
        public IQuestionUIController Question => _question;
        public bool IsModalUIShown => _modalInterfaces.Any(modalUI => modalUI.IsShown);

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

            _modalInterfacesToReshow = new List<IModalUI>();
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

        public void ResetModalUIs()
        {
            _modalInterfacesToReshow.Clear();
        }

        public void HideModalInterfaces()
        {
            foreach (IModalUI modalUI in _modalInterfaces)
            {
                bool isSetToReshow = modalUI.SetInactive();
                if (isSetToReshow)
                {
                    _modalInterfacesToReshow.Add(modalUI);
                }
            }
        }

        public void ShowHiddenModalInterfaces()
        {
            foreach (IModalUI modalUI in _modalInterfacesToReshow)
            {
                modalUI.SetActive();
            }
            ResetModalUIs();
        }

        private void OnModalUIAppeared()
        {
            if (IsModalUIShown) return;
            ModalUIAppeared?.Invoke();
        }

        private void OnModalUIDisappeared()
        {
            if (IsModalUIShown) return;
            ModalUIDisappeared?.Invoke();
        }
    }
}