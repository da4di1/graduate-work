using System;
using System.Collections.Generic;
using System.Linq;
using Core.PlayerAccount.Interfaces;
using Core.Services.PlayFab;
using Core.UI.DialogUI;
using Core.UI.QuestionUI;
using Core.UI.WarehouseInventory;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.UI
{
    public class GameSessionMenuUIPresenter : MonoBehaviour
    {
        [SerializeField] private Transform _userInterface;
        [SerializeField] private Transform _gameOverInterface;
        [SerializeField] private Transform _loadingScreen;
        [SerializeField] private Transform _errorScreen;
        [SerializeField] private LayerMask _viewUIMask;
        
        [Header("Post-Process")]
        [SerializeField] private PostProcessVolume _postProcessVolume;
        [SerializeField] private PostProcessLayer _postProcessLayer;

        private PlayFabService _playFabService;
        private IPlayerInformation _playerInformation;
        private List<Button[]> _stopButtonsLayers;
        private List<Transform> _openedUIWindows;

        public string EnteredNickname => _playFabService.PlayerAccountNickname;
        
        public event Action GameUIHidden;
        public event Action GameUIShown;
        
        
        private void Awake()
        {
            _stopButtonsLayers = new List<Button[]>();
            _openedUIWindows = new List<Transform>();
            _playFabService = new PlayFabService();

            _playFabService.AccountInfoReceived += HideLoadingScreen;
            _playFabService.LeaderboardUpdated += ShowGameOverScreen;
            _playFabService.ErrorOccured += ShowErrorMessage;
        }

        private void Start()
        {
            QuestionUIController.Instance.QuestionAppeared += PauseInterface;
            QuestionUIController.Instance.QuestionDisappeared += UnPauseInterface;
            DialogUIController.Instance.DialogAppeared += PauseInterface;
            DialogUIController.Instance.DialogDisappeared += UnPauseInterface;
            WarehouseInventoryController.Instance.WarehouseInventoryAppeared += PauseInterface;
            WarehouseInventoryController.Instance.WarehouseInventoryDisappeared += UnPauseInterface;
        }

        private void OnDestroy()
        {
            _playFabService.AccountInfoReceived -= HideLoadingScreen;
            _playFabService.LeaderboardUpdated -= ShowGameOverScreen;
            _playFabService.ErrorOccured -= ShowErrorMessage;
            
            QuestionUIController.Instance.QuestionAppeared -= PauseInterface;
            QuestionUIController.Instance.QuestionDisappeared -= UnPauseInterface;
            DialogUIController.Instance.DialogAppeared -= PauseInterface;
            DialogUIController.Instance.DialogDisappeared -= UnPauseInterface;
            WarehouseInventoryController.Instance.WarehouseInventoryAppeared -= PauseInterface;
            WarehouseInventoryController.Instance.WarehouseInventoryDisappeared -= UnPauseInterface;
        }

        public void Initialize(IPlayerInformation playerInformation)
        {
            _playerInformation = playerInformation;
            ShowLoadingScreen();
            _playFabService.Initialize();
        }
        
        public void RestartGame()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            SceneManager.LoadScene(currentSceneIndex);
        }

        public void ReturnToMainMenu()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            int mainMenuSceneIndex = currentSceneIndex - 1;
            if (mainMenuSceneIndex < 0)
                return;

            SceneManager.LoadScene(mainMenuSceneIndex); 
        }

        public void ShowPlayerInformation()
        {
            if (DialogUIController.Instance.IsDialogUIShown) return;
            DialogUIController.Instance.ShowDialog($"PLAYER INFORMATION\n" +
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
            foreach (Transform windowUI in _userInterface)
            {
                if (!windowUI.gameObject.activeSelf) continue;
                _openedUIWindows.Add(windowUI);
                windowUI.gameObject.SetActive(false);
            }
            GameUIHidden?.Invoke();
        }

        public void ShowHiddenInterface()
        {
            foreach (var windowUI in _openedUIWindows)
            {
                windowUI.gameObject.SetActive(true);
            }
            _openedUIWindows.Clear();
            GameUIShown?.Invoke();
        }

        public void FinishGame()
        {
            ShowLoadingScreen();
            _playFabService.UpdateLeaderboard(300000);
        }

        private void ShowLoadingScreen()
        {
            HideInterface();
            _loadingScreen.gameObject.SetActive(true);
            _postProcessVolume.enabled = true;
            _postProcessLayer.enabled = true;
        }

        private void HideLoadingScreen()
        {
            ShowHiddenInterface();
            _loadingScreen.gameObject.SetActive(false);
            _postProcessVolume.enabled = false;
            _postProcessLayer.enabled = false;
        }

        private void ShowGameOverScreen()
        {
            _loadingScreen.gameObject.SetActive(false);
            _gameOverInterface.gameObject.SetActive(true);
        }
        
        private void ShowErrorMessage()
        {
            _loadingScreen.gameObject.SetActive(false);
            _errorScreen.gameObject.SetActive(true);
        }
        
        private void PauseInterface()
        {
            Button[] newButtonsLayer = FindObjectsOfType<Button>();
            foreach (var buttonsLayer in _stopButtonsLayers)
            {
                newButtonsLayer = newButtonsLayer.Except(buttonsLayer).ToArray();
            }
            _stopButtonsLayers.Add(newButtonsLayer);
            foreach (var button in newButtonsLayer)
            {
                if ((_viewUIMask.value & (1 << button.gameObject.layer)) != 0) continue; //checking if button layer included in layer mask
                button.interactable = false;
            }
        }

        private void UnPauseInterface()
        {
            Button[] lastButtonLayer = _stopButtonsLayers.ElementAtOrDefault(_stopButtonsLayers.Count - 1);
            if (lastButtonLayer == null) return;
            foreach (var button in lastButtonLayer)
            {
                if ((_viewUIMask.value & (1 << button.gameObject.layer)) != 0) continue; //checking if button layer included in layer mask
                button.interactable = true;
            }
            _stopButtonsLayers.Remove(lastButtonLayer);
        }
    }
}
