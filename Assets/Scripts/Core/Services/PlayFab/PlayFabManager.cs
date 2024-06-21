using System;
using System.Collections.Generic;
using Core.Enums;
using Core.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Core.Services.PlayFab
{
    public class PlayFabManager : MonoBehaviour
    {
        public static PlayFabManager Instance { get; private set; }

        [SerializeField] private MainMenuUIController _mainMenuUIController;

        private bool _isLoginErrorOccuring;
        private bool _isLoginUpdatingNicknameErrorOccuring;
        private bool _isUpdatingLeaderboardErrorOccuring;
        private bool _isGettingLeaderboardErrorOccuring;
        private int _statValue;

        public event Action<GameStartingScreenType> SuccessfullyLogged;
        public event Action NicknameSubmitted;
        public event Action GotLeaderboard;
        public event Action LeaderboardUpdated;
        public event Action NotAvailableNicknameErrorOccured;
        public event Action ErrorOccured;
        
        private void Awake()
        {
            Instance = this;
            
            Login();
        }
        
        public void Login()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true,
                }
            };
            PlayFabClientAPI.LoginWithCustomID(request, OnSuccessfullyLogged, OnLoginErrorOccured);
        }

        public void SubmitNickname()
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = _mainMenuUIController.EnteredNickname.text,
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnNicknameSubmitted, OnSubmittingNicknameErrorOccured);
        }
        
        public void GetLeaderboard()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "PlayersIncome",
                StartPosition = 0,
                MaxResultsCount = 10,
            };
            PlayFabClientAPI.GetLeaderboard(request, OnGotLeaderboard, OnGetLeaderboardErrorOccured);
        }

        public void UpdateLeaderboard(int statValue)
        {
            _statValue = statValue;
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "PlayersIncome",
                        Value = statValue,
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdated, OnUpdateLeaderboardErrorOccured);
        }

        public void RepeatServerActions()
        {
            if (_isLoginErrorOccuring) Login();
            if (_isUpdatingLeaderboardErrorOccuring) UpdateLeaderboard(_statValue);
            if (_isGettingLeaderboardErrorOccuring) GetLeaderboard();
            if (_isLoginUpdatingNicknameErrorOccuring) SubmitNickname();
        }
        
        private void OnSuccessfullyLogged(LoginResult result)
        {
            string nickname = null;
            if (result.InfoResultPayload.PlayerProfile != null) nickname = result.InfoResultPayload.PlayerProfile.DisplayName;

            Debug.Log("Account successfully logged-in/created!");
            SuccessfullyLogged?.Invoke(nickname == null ? GameStartingScreenType.EnteringNicknameWindow : GameStartingScreenType.MainMenu);
            _isLoginErrorOccuring = false;
        }

        private void OnNicknameSubmitted(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("Player`s nickname has been saved!");
            NicknameSubmitted?.Invoke();
            _isLoginUpdatingNicknameErrorOccuring = false;
        }
        
        private void OnGotLeaderboard(GetLeaderboardResult result)
        {
            _mainMenuUIController.UpdateLeaderboard(result.Leaderboard);
            
            Debug.Log("Leaderboard has been successfully received from server!");
            GotLeaderboard?.Invoke();
            _isGettingLeaderboardErrorOccuring = false;
        }
        
        private void OnLeaderboardUpdated(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Leaderboard successfully updated!");
            LeaderboardUpdated?.Invoke();
            _isUpdatingLeaderboardErrorOccuring = false;
        }

        private void OnLoginErrorOccured(PlayFabError error)
        {
            OnErrorOccured(error);
            _isLoginErrorOccuring = true;
        }

        private void OnSubmittingNicknameErrorOccured(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.NameNotAvailable || error.Error == PlayFabErrorCode.InvalidParams)
            {
                NotAvailableNicknameErrorOccured?.Invoke();
            }
            else
            {
                OnErrorOccured(error);
                _isLoginUpdatingNicknameErrorOccuring = true;
            }
        }
        
        private void OnUpdateLeaderboardErrorOccured(PlayFabError error)
        {
            OnErrorOccured(error);
            _isUpdatingLeaderboardErrorOccuring = true;
        }
        
        private void OnGetLeaderboardErrorOccured(PlayFabError error)
        {
            OnErrorOccured(error);
            _isGettingLeaderboardErrorOccuring = true;
        }

        private void OnErrorOccured(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
            ErrorOccured?.Invoke();
        }
    }
}