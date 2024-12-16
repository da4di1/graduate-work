using Core.PlayerAccount.Interfaces;
using Core.Services.PlayFab;
using Core.UI.DialogUI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class GameSessionMenuUIController : MonoBehaviour
    {
        [SerializeField] private Transform _gameOverInterface;
        [SerializeField] private Transform _loadingScreen;
        [SerializeField] private Transform _errorScreen;

        private IPlayerInformation _playerInformation;
        
        
        private void Start()
        {
            PlayFabManager.EventsManager.LeaderboardUpdated += ShowGameOverScreen;
            PlayFabManager.EventsManager.ErrorOccured += ShowErrorMessage;
        }

        private void OnDestroy()
        {
            PlayFabManager.EventsManager.LeaderboardUpdated -= ShowGameOverScreen;
            PlayFabManager.EventsManager.ErrorOccured -= ShowErrorMessage;
        }

        public void Initialize(IPlayerInformation playerInformation)
        {
            _playerInformation = playerInformation;
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
            PlayFabManager.OperationsManager.RepeatServerActions();
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
    }
}
