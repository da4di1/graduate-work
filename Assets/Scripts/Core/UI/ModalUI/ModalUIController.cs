using System;
using System.Collections.Generic;
using Core.UI.ModalUI.DialogUI;
using Core.UI.ModalUI.QuestionUI;
using UnityEngine;

namespace Core.UI.ModalUI
{
    public class ModalUIController : MonoBehaviour
    {
        public static ModalUIController Instance { get; private set; }

        [field: SerializeField] public DialogUIController Dialog { get; private set; }
        [field: SerializeField] public QuestionUIController Question { get; private set; }

        private List<IModalUI> _modalInterfaces;
        
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
                Dialog,
                Question,
            };
            
            HideModalInterfaces();
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

        private void OnModalUIAppeared()
        {
            ModalUIAppeared?.Invoke();
            IsModalUIShown = true;
        }

        private void OnModalUIDisappeared()
        {
            IsModalUIShown = false;
            ModalUIDisappeared?.Invoke();
        }

        private void HideModalInterfaces()
        {
            foreach (var modalUI in _modalInterfaces)
            {
                modalUI.Hide();
            }
        }
    }
}