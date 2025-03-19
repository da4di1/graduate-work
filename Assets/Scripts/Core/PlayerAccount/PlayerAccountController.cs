using System.Collections.Generic;
using WarehousingSystem.Interfaces;

namespace Core.PlayerAccount
{
    public class PlayerAccountController
    {
        private readonly int _startingMoneyAmount;
        private List<IWarehouseInformation> _warehouses;
        
        public string NickName { get; private set; }
        public int MoneyAmount { get; }
        public int ActiveContractsAmount { get; }
        public int Income => MoneyAmount - _startingMoneyAmount;


        public PlayerAccountController(int startingMoneyAmount)
        {
            _startingMoneyAmount = startingMoneyAmount;
            MoneyAmount = startingMoneyAmount;
            _warehouses = new List<IWarehouseInformation>();
        }

        public void SetNickName(string nickname)
        {
            NickName = nickname;
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