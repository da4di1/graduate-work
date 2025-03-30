using System.Collections.Generic;
using Core.Services.PlayFab;
using Core.UI.ModalUI;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class MainMenuUIPresenter : MonoBehaviour
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

        private PlayFabService _playFabService;

        [field: SerializeField] public TMP_InputField EnteredNickname { get; private set; }
        
        
        private void Awake()
        {
            _playFabService = new PlayFabService();
            
            _playFabService.AccountInfoReceived += ShowStartingMenu;
            _playFabService.NicknameSubmitted += ShowMainMenu;
            _playFabService.LeaderboardReceived += ShowLeaderboard;
            _playFabService.NotAvailableNicknameErrorOccured += ShowNicknameErrorMessage;
            _playFabService.ErrorOccured += ShowErrorMessage;
        }
        
        private void Start()
        {
            ModalUIController.Instance.ResetModalUIs();
            _loadingScreen.gameObject.SetActive(true);
            _playFabService.Initialize();
        }
        
        private void OnDestroy()
        {
            _playFabService.AccountInfoReceived -= ShowStartingMenu;
            _playFabService.NicknameSubmitted -= ShowMainMenu;
            _playFabService.LeaderboardReceived -= ShowLeaderboard;
            _playFabService.NotAvailableNicknameErrorOccured -= ShowNicknameErrorMessage;
            _playFabService.ErrorOccured -= ShowErrorMessage;
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

        public void SubmitNickname()
        {
            _playFabService.SubmitNickname(EnteredNickname.text);
        }

        public void GetLeaderboard()
        {
            _playFabService.GetLeaderboard();
        }
        
        public void TryAvoidError()
        {
            _playFabService.RepeatServerActions();
        }

        private void ShowStartingMenu()
        {
            _loadingScreen.gameObject.SetActive(false);
            if (_playFabService.PlayerAccountNickname == null)
            {
                _enteringNicknameWindow.gameObject.SetActive(true);
            }
            else
            {
                _mainMenuInterface.gameObject.SetActive(true);
            }
        }

        private void ShowMainMenu()
        {
            _loadingScreen.gameObject.SetActive(false);
            _mainMenuInterface.gameObject.SetActive(true);
        }

        private void ShowLeaderboard(List<PlayerLeaderboardEntry> table)
        {
            UpdateLeaderboard(table);
            _loadingScreen.gameObject.SetActive(false);
            _leaderboardInterface.gameObject.SetActive(true);
        }
        
        private void UpdateLeaderboard(List<PlayerLeaderboardEntry> table)
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