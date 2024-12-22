using System;
using System.Collections.Generic;
using Core.Enums;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Core.Services.PlayFab
{
    public class PlayFabService
    {
        private int _statValue;
        private bool _isOldSessionExpired;
        private string _suggestedAccountNickname;
        private PlayFabErrorType _currentErrorType;
        
        public string PlayerAccountNickname { get; private set; }
        
        public event Action AccountInfoReceived;
        public event Action<GameStartingScreenType> SuccessfullyLogged;
        public event Action NicknameSubmitted;
        public event Action<List<PlayerLeaderboardEntry>> LeaderboardReceived;
        public event Action LeaderboardUpdated;
        public event Action NotAvailableNicknameErrorOccured;
        public event Action ErrorOccured;
        
        
        public void Initialize()
        {
            GetAccountInfo();
        }
        
        public void SubmitNickname(string nickname)
        {
            _suggestedAccountNickname = nickname;
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
            switch (_currentErrorType)
            {
                case PlayFabErrorType.GeetingAccountInfoError:
                    GetAccountInfo();
                    break;
                case PlayFabErrorType.LoginError:
                    Login();
                    break;
                case PlayFabErrorType.UpdatingNicknameError:
                    SubmitNickname(_suggestedAccountNickname);
                    break;
                case PlayFabErrorType.GettingLeaderboardError:
                    GetLeaderboard();
                    break;
                case PlayFabErrorType.UpdatingLeaderboardError:
                    UpdateLeaderboard(_statValue);
                    break;
                case PlayFabErrorType.None:
                default:
                    break;
            }
        }

        private void GetAccountInfo()
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                var request = new GetAccountInfoRequest();
                PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoReceived, OnGettingAccountInfoErrorOccured);
            }
            else
            {
                Login();
            }
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

        private void OnAccountInfoReceived(GetAccountInfoResult result)
        {
            PlayerAccountNickname = result.AccountInfo.TitleInfo.DisplayName;
            
            Debug.Log("Player`s info has been received!");
            AccountInfoReceived?.Invoke();
            _currentErrorType = PlayFabErrorType.None;
        }
        
        private void OnSuccessfullyLogged(LoginResult result)
        {
            PlayerAccountNickname = null;
            if (result.InfoResultPayload.PlayerProfile != null) PlayerAccountNickname = result.InfoResultPayload.PlayerProfile.DisplayName;

            Debug.Log("Account successfully logged-in/created!");
            if (_isOldSessionExpired)
            {
                _isOldSessionExpired = false;
            }
            else
            {
                SuccessfullyLogged?.Invoke(PlayerAccountNickname == null ? GameStartingScreenType.EnteringNicknameWindow : GameStartingScreenType.MainMenu);
                _currentErrorType = PlayFabErrorType.None;
            }
        }

        private void OnNicknameSubmitted(UpdateUserTitleDisplayNameResult result)
        {
            PlayerAccountNickname = result.DisplayName;
            
            Debug.Log("Player`s nickname has been saved!");
            NicknameSubmitted?.Invoke();
            _currentErrorType = PlayFabErrorType.None;
        }
        
        private void OnLeaderboardReceived(GetLeaderboardResult result)
        {
            Debug.Log("Leaderboard has been successfully received from server!");
            LeaderboardReceived?.Invoke(result.Leaderboard);
            _currentErrorType = PlayFabErrorType.None;
        }
        
        private void OnLeaderboardUpdated(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Leaderboard successfully updated!");
            LeaderboardUpdated?.Invoke();
            _currentErrorType = PlayFabErrorType.None;
        }

        private void OnGettingAccountInfoErrorOccured(PlayFabError error)
        {
            _currentErrorType = PlayFabErrorType.GeetingAccountInfoError;
            OnErrorOccured(error);
        }

        private void OnLoginErrorOccured(PlayFabError error)
        {
            _currentErrorType = PlayFabErrorType.LoginError;
            OnErrorOccured(error);
        }

        private void OnSubmittingNicknameErrorOccured(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.NameNotAvailable || error.Error == PlayFabErrorCode.InvalidParams)
            {
                NotAvailableNicknameErrorOccured?.Invoke();
            }
            else
            {
                _currentErrorType = PlayFabErrorType.UpdatingNicknameError;
                OnErrorOccured(error);
            }
        }
        
        private void OnGettingLeaderboardErrorOccured(PlayFabError error)
        {
            _currentErrorType = PlayFabErrorType.GettingLeaderboardError;
            OnErrorOccured(error);
        }
        
        private void OnUpdateLeaderboardErrorOccured(PlayFabError error)
        {
            _currentErrorType = PlayFabErrorType.UpdatingLeaderboardError;
            OnErrorOccured(error);
        }
        
        private void OnErrorOccured(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.InvalidSessionTicket)
            {
                _isOldSessionExpired = true;
                Login();
                RepeatServerActions();
            }
            else
            {
                Debug.Log(error.GenerateErrorReport());
                ErrorOccured?.Invoke();
            }
        }
    }
}