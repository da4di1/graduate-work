using System;
using System.Collections.Generic;
using System.Linq;
using CarsSystem;
using CarsSystem.Controllers;
using CarsSystem.Enums;
using CarsSystem.Interfaces;
using Core.Services.Updater;
using Core.UI.ModalUI;
using Core.UI.WarehouseInventory;
using PathBuilding;
using UnityEngine;
using WarehousingSystem.Behaviour;
using WarehousingSystem.Data;
using WarehousingSystem.Interfaces;

namespace WarehousingSystem.Controllers
{
    public class WarehouseEntity : IWarehouseInformation, IDisposable
    {
        private readonly WarehouseScene _warehouseBehaviour;
        private readonly PathDrawer _pathDrawer;
        private bool _isPurchasable;
        private CarsFactory _carsFactory;
        
        public WarehouseDescriptor Descriptor { get; }
        public List<ICarInformation> Cars { get; }


        public WarehouseEntity(WarehouseDescriptor descriptor, WarehouseScene warehouseBehaviour, PathDrawer pathDrawer, CarsFactory carsFactory)
        {
            Descriptor = descriptor;
            _warehouseBehaviour = warehouseBehaviour;
            _isPurchasable = true;
            _pathDrawer = pathDrawer;
            _carsFactory = carsFactory;
            Cars = new List<ICarInformation>();

            ProjectUpdater.Instance.FixedUpdateCalled += OnFixedUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.FixedUpdateCalled -= OnFixedUpdate;
        }

        public int GetCarsAmount()
        {
            return Cars.Count;
        }

        public void BuyCar(CarType carType)
        {
            Vector3[] newVec = new Vector3[5];
            CarEntity car = _carsFactory.CreateCar(carType, newVec);
            Cars.Add(car);
        }
        
        public void SellCar(CarType carType)
        {
            ICarInformation carToSell = Cars.FirstOrDefault(car => car.Descriptor.CarType == carType);
            if (carToSell == null)
            {
                ModalUIController.Instance.Dialog.Show("You do not have any cars of this type!", null);
            }
            else
            {
                Cars.Remove(carToSell);
            }
            
        }

        public void FindAndRemoveCar()
        {
            
        }

        private void OnFixedUpdate()
        {
            if (_warehouseBehaviour.Clicked)
            {
                if (_isPurchasable)
                {
                    ModalUIController.Instance.Question.Show($"Do you want to buy this warehouse for {Descriptor.Cost}$?", () =>
                    {
                        _isPurchasable = false;
                        _warehouseBehaviour.GetPurchased();
                    }, null);
                }
                else
                {
                    WarehouseInventoryController.Instance.ShowWarehouseInventory(this, null, () =>
                    {
                        _isPurchasable = true;
                        _warehouseBehaviour.GetSold();
                    }, null);
                }
            }
            
            _warehouseBehaviour.ResetOneTimeActions();
        }
    }
}