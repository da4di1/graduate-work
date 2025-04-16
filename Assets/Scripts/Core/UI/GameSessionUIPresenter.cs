using System;
using System.Collections.Generic;
using System.Linq;
using Core.PlayerAccount;
using Core.Services.PlayFab;
using Core.UI.ModalUI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.UI
{
    public class GameSessionUIPresenter : MonoBehaviour
    {
        [SerializeField] private Transform _userInterface;
        [SerializeField] private Transform _pauseMenuScreen;
        [SerializeField] private Transform _gameOverScreen;
        [SerializeField] private Transform _loadingScreen;
        [SerializeField] private Transform _errorScreen;
        [SerializeField] private LayerMask _viewUIMask;
        
        [Header("Post-Process")]
        [SerializeField] private PostProcessVolume _postProcessVolume;
        [SerializeField] private PostProcessLayer _postProcessLayer;

        private PlayFabService _playFabService;
        private PlayerAccountController _playerInformation;
        private List<Transform> _interfacesToReshow;
        private List<Button> _modalUIAffectedButtons;
        
        public event Action GameUIHidden;
        public event Action GameUIShown;
        
        
        private void Awake()
        {
            _playFabService = new PlayFabService();
            _interfacesToReshow = new List<Transform>();
            _modalUIAffectedButtons = new List<Button>();
            List<Button> buttonsOnScene = _userInterface.GetComponentsInChildren<Button>(true).ToList();
            foreach (var button in buttonsOnScene)
            {
                if ((_viewUIMask.value & (1 << button.gameObject.layer)) != 0) continue; //checking if button layer included in layer mask
                _modalUIAffectedButtons.Add(button);
            }

            _playFabService.AccountInfoReceived += StartGameSession;
            _playFabService.LeaderboardUpdated += ShowGameOverScreen;
            _playFabService.ErrorOccured += ShowErrorMessage;
        }

        private void Start()
        {
            ModalUIController.Instance.ResetModalUIs();
            ShowLoadingScreen();
            _playFabService.Initialize();
            
            ModalUIController.Instance.ModalUIAppeared += PauseButtons;
            ModalUIController.Instance.ModalUIDisappeared += UnPauseButtons;
        }

        private void OnDestroy()
        {
            _playFabService.AccountInfoReceived -= StartGameSession;
            _playFabService.LeaderboardUpdated -= ShowGameOverScreen;
            _playFabService.ErrorOccured -= ShowErrorMessage;
            
            ModalUIController.Instance.ModalUIAppeared -= PauseButtons;
            ModalUIController.Instance.ModalUIDisappeared -= UnPauseButtons;
        }

        public void Initialize(PlayerAccountController playerInformation)
        {
            _playerInformation = playerInformation;
        }
        
        public void RestartGameSession()
        {
            ModalUIController.Instance.Question.Show("Are you sure you want to restart this game session?", () =>
            {
                ShowLoadingScreen();
                
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex);
            }, null);
        }

        public void ReturnToMainMenu()
        {
            ModalUIController.Instance.Question.Show("Are you sure you want to return to main menu?", () =>
            {
                ShowLoadingScreen();
                
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int mainMenuSceneIndex = currentSceneIndex - 1;
                if (mainMenuSceneIndex < 0) return;
                SceneManager.LoadScene(mainMenuSceneIndex); 
            }, null);
        }

        public void ShowPlayerInformation()
        {
            ModalUIController.Instance.Dialog.Show($"PLAYER INFORMATION\n" +
                                                     $"Nickname: {_playerInformation.NickName}\n" +
                                                     $"Money: {_playerInformation.MoneyAmount}$\n" +
                                                     $"Income: {_playerInformation.Income}$\n" +
                                                     $"Warehouses: {_playerInformation.GetWarehousesAmount()}\n" +
                                                     $"Cars: {_playerInformation.GetCarsAmount()}\n" +
                                                     $"Active contracts: {_playerInformation.ActiveContractsAmount}", null);
        }
        
        public void TryAvoidError()
        {
            _playFabService.RepeatServerActions();
        }

        public void HideInterface()
        {
            GameUIHidden?.Invoke();
            
            ModalUIController.Instance.HideModalInterfaces();
            foreach (Transform windowUI in _userInterface)
            {
                if (!windowUI.gameObject.activeSelf) continue;
                _interfacesToReshow.Add(windowUI);
                windowUI.gameObject.SetActive(false);
            }
            
            _postProcessVolume.enabled = true;
            _postProcessLayer.enabled = true;
        }

        public void ShowHiddenInterface()
        {
            GameUIShown?.Invoke();
            
            ModalUIController.Instance.ShowHiddenModalInterfaces();
            foreach (Transform windowUI in _interfacesToReshow)
            {
                windowUI.gameObject.SetActive(true);
            }
            _interfacesToReshow.Clear();
            
            _postProcessVolume.enabled = false;
            _postProcessLayer.enabled = false;
        }

        public void FinishGameSession()
        {
            ShowLoadingScreen();
            _playFabService.UpdateLeaderboard(300000);
        }
        
        private void StartGameSession()
        {
            _playerInformation.SetNickName(_playFabService.PlayerAccountNickname);
            HideLoadingScreen();
        }

        private void ShowLoadingScreen()
        {
            HideInterface();
            _loadingScreen.gameObject.SetActive(true);
        }

        private void HideLoadingScreen()
        {
            ShowHiddenInterface();
            _loadingScreen.gameObject.SetActive(false);
        }

        private void ShowGameOverScreen()
        {
            _loadingScreen.gameObject.SetActive(false);
            _gameOverScreen.gameObject.SetActive(true);
        }
        
        private void ShowErrorMessage()
        {
            _loadingScreen.gameObject.SetActive(false);
            _errorScreen.gameObject.SetActive(true);
        }
        
        private void PauseButtons()
        {
            foreach (var button in _modalUIAffectedButtons)
            {
                button.interactable = false;
            }
        }

        private void UnPauseButtons()
        {
            foreach (var button in _modalUIAffectedButtons)
            {
                button.interactable = true;
            }
        }
    }
}