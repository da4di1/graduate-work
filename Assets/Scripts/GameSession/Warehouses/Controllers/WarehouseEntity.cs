using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI;
using Core.Services.Updater;
using GameSession.Cars;
using GameSession.Cars.Controllers;
using GameSession.Cars.Enums;
using GameSession.Cars.Interfaces;
using GameSession.UI.WarehouseInventory.Interfaces;
using GameSession.Warehouses.Behaviour;
using GameSession.Warehouses.Data;
using GameSession.Warehouses.Interfaces;
using UnityEngine;

namespace GameSession.Warehouses.Controllers
{
    public class WarehouseEntity : IWarehouseInformation/*, IDisposable*/
    {
        /*private readonly SceneWarehouse _sceneWarehouseBehaviour;*/
        /*private readonly IWarehouseInventoryUIDisplayer _warehouseInventory;*/
        private readonly CarsFactory _carsFactory;
        private readonly List<ICarInformation> _cars;
        
        public bool IsPurchasable { get; private set; }
        public WarehouseDescriptor Descriptor { get; }


        public WarehouseEntity(WarehouseDescriptor descriptor, CarsFactory carsFactory)
        {
            Descriptor = descriptor;
            /*_sceneWarehouseBehaviour = sceneWarehouseBehaviour;
            _sceneWarehouseBehaviour.Initialize((IWarehouseInventoryState)warehouseInventory);
            _warehouseInventory = warehouseInventory;*/
            _carsFactory = carsFactory;
            IsPurchasable = true;
            _cars = new List<ICarInformation>();

            /*ProjectUpdater.Instance.UpdateCalled += OnUpdate;*/
        }

        /*public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }*/

        public int GetCarsAmount()
        {
            return _cars.Count;
        }
        
        public int GetCarsAmount(CarType carType)
        {
            return _cars.Count(car => car.Descriptor.Type == carType);
        }

        public void GetPurchased()
        {
            IsPurchasable = false;
        }
        
        public void GetSold()
        {
            IsPurchasable = true;
        }

        public void BuyCar(CarType carType)
        {
            Vector3[] newVec = new Vector3[5];
            CarEntity car = _carsFactory.CreateCar(carType, newVec);
            _cars.Add(car);
        }
        
        public void SellCar(CarType carType)
        {
            ICarInformation carToSell = _cars.FirstOrDefault(car => car.Descriptor.Type == carType);
            if (carToSell == null)
            {
                ModalUIController.Instance.Dialog.Show("You do not have any cars of this type!", null);
            }
            else
            {
                _cars.Remove(carToSell);
            }
        }

        /*private void OnUpdate()
        {
            if (_sceneWarehouseBehaviour.Clicked)
            {
                if (_isPurchasable)
                {
                    ModalUIController.Instance.Question.Show($"Do you want to buy this warehouse for {Descriptor.Cost}$?", () =>
                    {
                        _isPurchasable = false;
                        _sceneWarehouseBehaviour.GetPurchased();
                    }, null);
                }
                else
                {
                    _warehouseInventory.Show(this, null, () =>
                    {
                        _isPurchasable = true;
                        _sceneWarehouseBehaviour.GetSold();
                    }, null);
                }
            }
            
            _sceneWarehouseBehaviour.ResetOneTimeActions();
        }*/
    }
}