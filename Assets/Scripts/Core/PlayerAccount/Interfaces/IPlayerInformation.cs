namespace Core.PlayerAccount.Interfaces
{
    public interface IPlayerInformation
    {
        string NickName { get; }
        int MoneyAmount { get; }
        int Income { get; }
        int ActiveContractsAmount { get; }
        int GetWarehousesAmount();
        int GetCarsAmount();
    }
}