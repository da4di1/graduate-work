using System;
using System.Collections.Generic;
using Core.Enums;
using PlayFab.ClientModels;

namespace Core.Services.PlayFab
{
    public interface IPlayFabEventsManager
    {
        event Action<GameStartingScreenType> SuccessfullyLogged;
        event Action NicknameSubmitted;
        event Action<List<PlayerLeaderboardEntry>> LeaderboardReceived;
        event Action LeaderboardUpdated;
        event Action NotAvailableNicknameErrorOccured;
        event Action ErrorOccured;
    }
}