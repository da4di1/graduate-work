using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI;
using Core.Services.Updater;
using GameSession.Cars;
using GameSession.PlayerAccount;
using GameSession.UI.WarehouseInventory.Controllers;
using GameSession.Warehouses.Behaviour;
using GameSession.Warehouses.Controllers;
using GameSession.Warehouses.Data;

namespace GameSession.Warehouses
{
    public class WarehousesSystem: IDisposable
    {
        private readonly Dictionary<SceneWarehouse, WarehouseEntity> _warehousesOnScene;
        private readonly WarehouseInventoryUIController _warehouseInventory;
        private readonly IPlayerWarehousesController _playerAccount;
        
        
        public WarehousesSystem(List<SceneWarehouse> sceneWarehouses, List<WarehouseDescriptor> warehousesDescriptors, 
            WarehouseInventoryUIController warehouseInventory, IPlayerWarehousesController playerAccount, CarsFactory carsFactory)
        {
            _warehouseInventory = warehouseInventory;
            _playerAccount = playerAccount;
            
            _warehousesOnScene = new Dictionary<SceneWarehouse, WarehouseEntity>();
            foreach (var sceneWarehouse in sceneWarehouses)
            {
                WarehouseDescriptor warehouseDescriptor = warehousesDescriptors.FirstOrDefault(descriptor => descriptor.Id == sceneWarehouse.WarehouseId);
                if (warehouseDescriptor == null) continue;
                
                WarehouseEntity warehouseEntity = new WarehouseEntity(warehouseDescriptor, carsFactory);
                _warehousesOnScene.Add(sceneWarehouse, warehouseEntity);
            }
            
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }
        
        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }

        private void OnUpdate()
        {
            foreach (var warehouseOnScene in _warehousesOnScene)
            {
                if (warehouseOnScene.Key.Clicked && !ProjectUpdater.Instance.IsPaused && !ModalUIController.Instance.IsModalUIShown 
                    && !_warehouseInventory.IsWarehouseInventoryUIShown)
                {
                    if (warehouseOnScene.Value.IsPurchasable)
                    {
                        ModalUIController.Instance.Question.Show($"Do you want to buy this warehouse for " +
                                                                 $"{warehouseOnScene.Value.Descriptor.Cost}$?", () =>
                        {
                            warehouseOnScene.Value.GetPurchased();
                            warehouseOnScene.Key.GetPurchased();
                            _playerAccount.AddWarehouse(warehouseOnScene.Value);
                        }, null);
                    }
                    else
                    {
                        _warehouseInventory.Show(warehouseOnScene.Value, null, () =>
                        {
                            warehouseOnScene.Value.GetSold();
                            warehouseOnScene.Key.GetSold();
                            _playerAccount.RemoveWarehouse(warehouseOnScene.Value);
                        }, null);
                    }
                }

                if (warehouseOnScene.Key.Hovered && !ProjectUpdater.Instance.IsPaused && !ModalUIController.Instance.IsModalUIShown
                    && !_warehouseInventory.IsWarehouseInventoryUIShown)
                {
                    warehouseOnScene.Key.EnableBackground();
                }
            
                warehouseOnScene.Key.ResetOneTimeActions();
            }
        }
    }
}