using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI;
using Core.Services.PlayFab;
using GameSession.PlayerAccount;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameSession.UI
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

        private bool _isGameSessionOver;
        private PlayFabService _playFabService;
        private PlayerAccountController _playerInformation;
        private Stack<List<Transform>> _interfacesLayersToReshow;
        private List<Button> _modalUIAffectedButtons;
        private List<Button> _buttonsToReactivate;
        
        public event Action GameUIHidden;
        public event Action GameUIShown;
        
        
        private void Awake()
        {
            _playFabService = new PlayFabService();
            _interfacesLayersToReshow = new Stack<List<Transform>>();
            _modalUIAffectedButtons = new List<Button>();
            _buttonsToReactivate = new List<Button>();
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
            ShowLoadingScreenExclusive();
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
            if (!_isGameSessionOver)
            {
                HideNonModalInterfaces();
                ModalUIController.Instance.Question.Show("Are you sure you want to restart this game session? \nYour data will NOT be saved.",
                    ReloadGameScene, ShowHiddenNonModalInterfaces);
            }
            else
            {
                ReloadGameScene();
            }
        }

        public void ReturnToMainMenu()
        {
            if (!_isGameSessionOver)
            {
                HideNonModalInterfaces();
                ModalUIController.Instance.Question.Show("Are you sure you want to return to the main menu? \nYour data will NOT be saved.",
                    LoadInitialGameScene, ShowHiddenNonModalInterfaces);
            }
            else
            {
                LoadInitialGameScene();
            }
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
            HideNonModalInterfaces();
            
            _postProcessVolume.enabled = true;
            _postProcessLayer.enabled = true;
        }

        public void ShowHiddenInterface()
        {
            GameUIShown?.Invoke();
            
            ModalUIController.Instance.ShowHiddenModalInterfaces();
            ShowHiddenNonModalInterfaces();
            
            _postProcessVolume.enabled = false;
            _postProcessLayer.enabled = false;
        }

        public void FinishGameSession()
        {
            ShowLoadingScreenExclusive();
            _playFabService.UpdateLeaderboard(300000);
        }
        
        private void StartGameSession()
        {
            _playerInformation.SetNickName(_playFabService.PlayerAccountNickname);
            RestoreUIAfterLoading();
        }

        private void ReloadGameScene()
        {
            ShowLoadingScreenExclusive();
                
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        private void LoadInitialGameScene()
        {
            ShowLoadingScreenExclusive();
                
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int mainMenuSceneIndex = currentSceneIndex - 1;
            if (mainMenuSceneIndex < 0) return;
            SceneManager.LoadScene(mainMenuSceneIndex); 
        }

        private void ShowLoadingScreenExclusive()
        {
            HideInterface();
            _loadingScreen.gameObject.SetActive(true);
        }

        private void RestoreUIAfterLoading()
        {
            ShowHiddenInterface();
            _loadingScreen.gameObject.SetActive(false);
        }

        private void ShowGameOverScreen()
        {
            _isGameSessionOver = true;
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
                if (button.interactable == false) continue;
                button.interactable = false;
                _buttonsToReactivate.Add(button);
            }
        }

        private void UnPauseButtons()
        {
            foreach (var button in _buttonsToReactivate)
            {
                button.interactable = true;
            }
            _buttonsToReactivate.Clear();
        }

        private void HideNonModalInterfaces()
        {
            List<Transform> interfacesLayer = new List<Transform>();
            foreach (Transform windowUI in _userInterface)
            {
                if (!windowUI.gameObject.activeSelf) continue;
                interfacesLayer.Add(windowUI);
                windowUI.gameObject.SetActive(false);
            }
            _interfacesLayersToReshow.Push(interfacesLayer);
        }
        
        private void ShowHiddenNonModalInterfaces()
        {
            if (_interfacesLayersToReshow.Count <= 0) return;
            List<Transform> interfacesLayer = _interfacesLayersToReshow.Pop();
            foreach (Transform windowUI in interfacesLayer)
            {
                windowUI.gameObject.SetActive(true);
            }
        }
    }
}