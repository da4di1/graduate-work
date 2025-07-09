using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI.ModalInterfaces.Dialog;
using Core.ModalUI.ModalInterfaces.Question;
using UnityEngine;

namespace Core.ModalUI
{
    public class ModalUIController : MonoBehaviour
    {
        public static ModalUIController Instance { get; private set; }

        [SerializeField] private DialogUIDisplayer _dialog;
        [SerializeField] private QuestionUIDisplayer _question;

        private List<IModalUI> _modalInterfaces;
        private List<IModalUI> _modalInterfacesToReshow;

        public IDialogUIDisplayer Dialog => _dialog;
        public IQuestionUIDisplayer Question => _question;
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
            HideModalInterfaces();
            _modalInterfacesToReshow.Clear();
        }

        public void HideModalInterfaces()
        {
            foreach (IModalUI modalUI in _modalInterfaces)
            {
                bool isSetToReshow = modalUI.Hide();
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
                modalUI.Reshow();
            }
            _modalInterfacesToReshow.Clear();
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