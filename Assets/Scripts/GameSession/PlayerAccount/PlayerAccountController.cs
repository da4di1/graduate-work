using System.Collections.Generic;
using GameSession.Warehouses.Interfaces;

namespace GameSession.PlayerAccount
{
    public class PlayerAccountController : IPlayerWarehousesController
    {
        private readonly int _startingMoneyAmount;
        private readonly List<IWarehouseInformation> _warehouses;
        
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

        public void AddWarehouse(IWarehouseInformation warehouse)
        {
            _warehouses.Add(warehouse);
        }
        
        public void RemoveWarehouse(IWarehouseInformation warehouse)
        {
            _warehouses.Remove(warehouse);
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