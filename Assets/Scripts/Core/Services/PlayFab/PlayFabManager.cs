using System;
using System.Collections.Generic;
using Core.Enums;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Core.Services.PlayFab
{
    public class PlayFabManager : IPlayFabEventsManager, IPlayFabOperationsManager
    {
        public static IPlayFabEventsManager EventsManager { get; private set; }
        public static IPlayFabOperationsManager OperationsManager { get; private set; }

        private bool _isLoginErrorOccuring;
        private bool _isLoginUpdatingNicknameErrorOccuring;
        private bool _isUpdatingLeaderboardErrorOccuring;
        private bool _isGettingLeaderboardErrorOccuring;
        private int _statValue;
        
        public string ReceivedPlayerAccountNickname { get; private set; }

        public event Action<GameStartingScreenType> SuccessfullyLogged;
        public event Action NicknameSubmitted;
        public event Action<List<PlayerLeaderboardEntry>> LeaderboardReceived;
        public event Action LeaderboardUpdated;
        public event Action NotAvailableNicknameErrorOccured;
        public event Action ErrorOccured;

        
        public PlayFabManager()
        {
            if (EventsManager != null || OperationsManager != null) return;
            EventsManager = this;
            OperationsManager = this;
            
            Login();
        }

        public void SubmitNickname(string nickname)
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = nickname,
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
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardReceived, OnGettingLeaderboardErrorOccured);
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
            if (_isLoginUpdatingNicknameErrorOccuring) SubmitNickname(ReceivedPlayerAccountNickname);
        }
        
        private void Login()
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
        
        private void OnSuccessfullyLogged(LoginResult result)
        {
            ReceivedPlayerAccountNickname = null;
            if (result.InfoResultPayload.PlayerProfile != null) ReceivedPlayerAccountNickname = result.InfoResultPayload.PlayerProfile.DisplayName;

            Debug.Log("Account successfully logged-in/created!");
            SuccessfullyLogged?.Invoke(ReceivedPlayerAccountNickname == null ? GameStartingScreenType.EnteringNicknameWindow : GameStartingScreenType.MainMenu);
            _isLoginErrorOccuring = false;
        }

        private void OnNicknameSubmitted(UpdateUserTitleDisplayNameResult result)
        {
            ReceivedPlayerAccountNickname = result.DisplayName;
            
            Debug.Log("Player`s nickname has been saved!");
            NicknameSubmitted?.Invoke();
            _isLoginUpdatingNicknameErrorOccuring = false;
        }
        
        private void OnLeaderboardReceived(GetLeaderboardResult result)
        {
            Debug.Log("Leaderboard has been successfully received from server!");
            LeaderboardReceived?.Invoke(result.Leaderboard);
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
        
        private void OnGettingLeaderboardErrorOccured(PlayFabError error)
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