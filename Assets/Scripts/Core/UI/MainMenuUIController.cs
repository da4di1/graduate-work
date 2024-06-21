using System.Collections.Generic;
using Core.Enums;
using Core.Services.PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        [Header("Starting screens")] 
        [SerializeField] private Transform _mainMenuInterface;
        [SerializeField] private Transform _enteringNicknameWindow;
        [SerializeField] private Transform _nicknameWarningMessage;
        [SerializeField] private Transform _nicknameErrorMessage;
        
        [Header("Leaderboard")] 
        [SerializeField] private Transform _rowUI;
        [SerializeField] private Transform _tableUI;
        [SerializeField] private Transform _leaderboardInterface;
        
        [Header("Server connections screens")] 
        [SerializeField] private Transform _loadingScreen;
        [SerializeField] private Transform _errorScreen;

        [field: SerializeField] public TMP_InputField EnteredNickname { get; private set; }
        
        
        private void Awake()
        {
            _loadingScreen.gameObject.SetActive(true);
        }

        private void Start()
        {
            PlayFabManager.Instance.SuccessfullyLogged += ShowStartingMenu;
            PlayFabManager.Instance.NicknameSubmitted += ShowMainMenu;
            PlayFabManager.Instance.GotLeaderboard += ShowLeaderboard;
            PlayFabManager.Instance.NotAvailableNicknameErrorOccured += ShowNicknameErrorMessage;
            PlayFabManager.Instance.ErrorOccured += ShowErrorMessage;
        }

        private void OnDestroy()
        {
            PlayFabManager.Instance.SuccessfullyLogged -= ShowStartingMenu;
            PlayFabManager.Instance.NicknameSubmitted -= ShowMainMenu;
            PlayFabManager.Instance.GotLeaderboard -= ShowLeaderboard;
            PlayFabManager.Instance.NotAvailableNicknameErrorOccured -= ShowNicknameErrorMessage;
            PlayFabManager.Instance.ErrorOccured -= ShowErrorMessage;
        }

        public void StartGame()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            int gameSceneIndex = currentSceneIndex + 1;
            if (gameSceneIndex == SceneManager.sceneCountInBuildSettings)
                return;

            SceneManager.LoadScene(gameSceneIndex); 
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }

        public void UpdateLeaderboard(List<PlayerLeaderboardEntry> table)
        {
            foreach (Transform row in _tableUI)
            {
                Destroy(row.gameObject);
            }

            foreach (var row in table)
            {
                Transform newRow = Instantiate(_rowUI, _tableUI);
                TextMeshProUGUI[] columns = newRow.GetComponentsInChildren<TextMeshProUGUI>();
                columns[0].text = (row.Position + 1).ToString();
                columns[1].text = row.DisplayName;
                columns[2].text = row.StatValue.ToString();
            }
        }
        
        public void TryAvoidError()
        {
            PlayFabManager.Instance.RepeatServerActions();
        }

        private void ShowStartingMenu(GameStartingScreenType startingScreenType)
        {
            _loadingScreen.gameObject.SetActive(false);
            switch (startingScreenType)
            {
                case GameStartingScreenType.EnteringNicknameWindow:
                    _enteringNicknameWindow.gameObject.SetActive(true);
                    break;
                case GameStartingScreenType.MainMenu:
                    _mainMenuInterface.gameObject.SetActive(true);
                    break;
            }
        }

        private void ShowMainMenu()
        {
            _loadingScreen.gameObject.SetActive(false);
            _mainMenuInterface.gameObject.SetActive(true);
        }

        private void ShowLeaderboard()
        {
            _loadingScreen.gameObject.SetActive(false);
            _leaderboardInterface.gameObject.SetActive(true);
        }
        
        private void ShowNicknameErrorMessage()
        {
            _nicknameWarningMessage.gameObject.SetActive(false);
            _nicknameErrorMessage.gameObject.SetActive(true);
            _loadingScreen.gameObject.SetActive(false);
            _enteringNicknameWindow.gameObject.SetActive(true);
        }

        private void ShowErrorMessage()
        {
            _loadingScreen.gameObject.SetActive(false);
            _errorScreen.gameObject.SetActive(true);
        }
    }
}
