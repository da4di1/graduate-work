namespace Core.Services.PlayFab
{
    public interface IPlayFabOperationsManager
    {
        string ReceivedPlayerAccountNickname { get; }
        void SubmitNickname(string nickname);
        void GetLeaderboard();
        void UpdateLeaderboard(int statValue);
        void RepeatServerActions();
    }
}