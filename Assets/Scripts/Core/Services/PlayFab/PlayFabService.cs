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
        public event Action NicknameSubmitted;
        public event Action<List<PlayerLeaderboardEntry>> LeaderboardReceived;
        public event Action LeaderboardUpdated;
        public event Action NotAvailableNicknameErrorOccured;
        public event Action ErrorOccured;
        
        
        public void Initialize()
        {
            CheckLoggingStatus();
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
                case PlayFabErrorType.GettingAccountInfoError:
                    GetAccountInfo();
                    break;
                case PlayFabErrorType.LoginError:
                    LogIn();
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

        private void CheckLoggingStatus()
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                GetAccountInfo();
            }
            else
            {
                LogIn();
            }
        }

        private void GetAccountInfo()
        {
            var request = new GetAccountInfoRequest();
            PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoReceived, OnGettingAccountInfoErrorOccured);
        }
        
        private void LogIn()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
            };
            PlayFabClientAPI.LoginWithCustomID(request, OnSuccessfullyLogged, OnLoginErrorOccured);
        }

        private void OnAccountInfoReceived(GetAccountInfoResult result)
        {
            PlayerAccountNickname = null;
            if (result.AccountInfo.TitleInfo != null) PlayerAccountNickname = result.AccountInfo.TitleInfo.DisplayName;
            
            Debug.Log("Player`s info has been received!");
            _currentErrorType = PlayFabErrorType.None;
            AccountInfoReceived?.Invoke();
        }
        
        private void OnSuccessfullyLogged(LoginResult result)
        {
            Debug.Log("Account successfully logged-in/created!");
            if (_isOldSessionExpired)
            {
                _isOldSessionExpired = false;
            }
            else
            {
                _currentErrorType = PlayFabErrorType.None;
                GetAccountInfo();
            }
        }

        private void OnNicknameSubmitted(UpdateUserTitleDisplayNameResult result)
        {
            PlayerAccountNickname = result.DisplayName;
            
            Debug.Log("Player`s nickname has been saved!");
            _currentErrorType = PlayFabErrorType.None;
            NicknameSubmitted?.Invoke();
        }
        
        private void OnLeaderboardReceived(GetLeaderboardResult result)
        {
            Debug.Log("Leaderboard has been successfully received from server!");
            _currentErrorType = PlayFabErrorType.None;
            LeaderboardReceived?.Invoke(result.Leaderboard);
        }
        
        private void OnLeaderboardUpdated(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Leaderboard successfully updated!");
            _currentErrorType = PlayFabErrorType.None;
            LeaderboardUpdated?.Invoke();
        }

        private void OnGettingAccountInfoErrorOccured(PlayFabError error)
        {
            _currentErrorType = PlayFabErrorType.GettingAccountInfoError;
            OnErrorOccured(error);
        }

        private void OnLoginErrorOccured(PlayFabError error)
        {
            if (!_isOldSessionExpired) _currentErrorType = PlayFabErrorType.LoginError;
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
                LogIn();
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