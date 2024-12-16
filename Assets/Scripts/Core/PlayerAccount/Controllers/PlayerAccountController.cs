using System.Collections.Generic;
using Core.PlayerAccount.Interfaces;
using WarehousingSystem.Interfaces;

namespace Core.PlayerAccount.Controllers
{
    public class PlayerAccountController : IPlayerInformation
    {
        private readonly int _startingMoneyAmount;
        private List<IWarehouseInformation> _warehouses;
        
        public string NickName { get; }
        public int MoneyAmount { get; }
        public int ActiveContractsAmount { get; }
        public int Income => MoneyAmount - _startingMoneyAmount;


        public PlayerAccountController(string nickname, int startingMoneyAmount)
        {
            NickName = nickname;
            _startingMoneyAmount = startingMoneyAmount;
            MoneyAmount = startingMoneyAmount;
            _warehouses = new List<IWarehouseInformation>();
        }

        public int GetWarehousesAmount()
        {
            return _warehouses.Count;
        }

        public int GetCarsAmount()
        {
            int totalCarsAmount = 0;
            foreach (var warehouse in _warehouses)
            {
                totalCarsAmount += warehouse.GetCarsAmount();
            }
            
            return totalCarsAmount;
        }
    }
}