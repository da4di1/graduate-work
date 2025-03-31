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
using UnityEngine;
using WarehousingSystem.Behaviour;
using WarehousingSystem.Data;
using WarehousingSystem.Interfaces;

namespace WarehousingSystem.Controllers
{
    public class WarehouseEntity : IWarehouseInformation, IDisposable
    {
        private readonly WarehouseScene _warehouseBehaviour;
        private readonly IWarehouseInventoryUIDisplayer _warehouseInventory;
        private readonly CarsFactory _carsFactory;
        private bool _isPurchasable;
        
        public WarehouseDescriptor Descriptor { get; }
        public List<ICarInformation> Cars { get; }


        public WarehouseEntity(WarehouseDescriptor descriptor, WarehouseScene warehouseBehaviour, IWarehouseInventoryUIDisplayer warehouseInventory, CarsFactory carsFactory)
        {
            Descriptor = descriptor;
            _warehouseBehaviour = warehouseBehaviour;
            _warehouseBehaviour.Initialize((IWarehouseInventoryState)warehouseInventory);
            _warehouseInventory = warehouseInventory;
            _carsFactory = carsFactory;
            _isPurchasable = true;
            Cars = new List<ICarInformation>();

            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }

        public int GetCarsAmount()
        {
            return Cars.Count;
        }
        
        public int GetCarsAmount(CarType carType)
        {
            return Cars.Count(car => car.Descriptor.CarType == carType);
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

        private void OnUpdate()
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
                    _warehouseInventory.Show(this, null, () =>
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