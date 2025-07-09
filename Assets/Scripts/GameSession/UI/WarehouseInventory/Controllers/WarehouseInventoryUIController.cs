using System;
using System.Collections.Generic;
using System.Linq;
using Core.ModalUI;
using GameSession.Cars.Data;
using GameSession.PathBuilding;
using GameSession.UI.WarehouseInventory.Data;
using GameSession.UI.WarehouseInventory.Interfaces;
using GameSession.Warehouses.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace GameSession.UI.WarehouseInventory.Controllers
{
    public class WarehouseInventoryUIController : MonoBehaviour, IWarehouseInventoryUIDisplayer, IWarehouseInventoryState
    {
        [SerializeField] private LoadCarInventoryUIController _loadCarInventory;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _loadCarButton;
        [SerializeField] private List<WarehouseInventoryCarUIDescriptor> _carsUI;
        
        private List<CarDescriptor> _carsDescriptors;
        
        public bool IsWarehouseInventoryUIShown => gameObject.activeSelf || _loadCarInventory.gameObject.activeSelf;

        public event Action WarehouseInventoryAppeared;
        public event Action WarehouseInventoryDisappeared;
        

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Initialize(List<CarDescriptor> carsDescriptors, PathDrawer pathDrawer)
        {
            _loadCarInventory.Initialize(carsDescriptors, pathDrawer);
            _carsDescriptors = carsDescriptors;
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carDescriptor = carsDescriptors.FirstOrDefault(descriptor => descriptor.Type == carUI.CarType);
                if (carDescriptor == null) continue;
                
                carUI.Cost.text = carDescriptor.Cost + "$";
                carUI.SalePrice.text = carDescriptor.SalePrice + "$";
                carUI.Icon.sprite = carDescriptor.HorizontalSprite;
            }
        }

        public void Show(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked, Action loadCarButtonClicked)
        {
            WarehouseInventoryAppeared?.Invoke();
            gameObject.SetActive(true);
            SetCarsUI(warehouseEntity);
            
            _closeButton.onClick.AddListener(() =>
            {
                Hide();
                closeButtonClicked?.Invoke();
            });
            _sellButton.onClick.AddListener(() =>
            {
                ModalUIController.Instance.Question.Show($"Do you want to sell this warehouse for {warehouseEntity.Descriptor.SalePrice}$?",
                    () =>
                    {
                        if (warehouseEntity.GetCarsAmount() > 0)
                        {
                            ModalUIController.Instance.Dialog.Show("Firstly you must sell all the cars in the warehouse!", null);
                        }
                        else
                        {
                            Hide();
                            sellButtonClicked?.Invoke();
                        }
                    }, null);
            });
            _loadCarButton.onClick.AddListener(() =>
            {
                if (warehouseEntity.GetCarsAmount() > 0)
                {
                    gameObject.SetActive(false);
                    _loadCarInventory.Show(warehouseEntity, () => gameObject.SetActive(true), Hide);
                    loadCarButtonClicked?.Invoke();
                }
                else
                {
                    ModalUIController.Instance.Dialog.Show("You must have at least one car for this process!", null);
                }
            });
        }

        private void SetCarsUI(WarehouseEntity warehouseEntity)
        {
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carDescriptor = _carsDescriptors.FirstOrDefault(descriptor => descriptor.Type == carUI.CarType);
                if (carDescriptor == null) continue;
                
                carUI.Amount.text = warehouseEntity.GetCarsAmount(carUI.CarType).ToString();
                carUI.BuyButton.onClick.AddListener(() =>
                {
                    ModalUIController.Instance.Question.Show($"Do you want to buy this car for {carDescriptor.Cost}?\n" +
                                                             $"Speed: {carDescriptor.Speed} km/h\n" +
                                                             $"Capacity {carDescriptor.ProductCapacity} kg", () =>
                    {
                        warehouseEntity.BuyCar(carUI.CarType);
                        carUI.Amount.text = warehouseEntity.GetCarsAmount(carUI.CarType).ToString();
                    }, null);
                });
                carUI.SellButton.onClick.AddListener(() =>
                {
                    ModalUIController.Instance.Question.Show($"Do you want to sell this car for {carDescriptor.SalePrice}?\n" +
                                                             $"Speed: {carDescriptor.Speed} km/h\n" +
                                                             $"Capacity {carDescriptor.ProductCapacity} kg", () =>
                    {
                        warehouseEntity.SellCar(carUI.CarType);
                        carUI.Amount.text = warehouseEntity.GetCarsAmount(carUI.CarType).ToString();
                    }, null);
                });
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            RemoveButtonsListeners();
            WarehouseInventoryDisappeared?.Invoke();
        }

        private void RemoveButtonsListeners()
        {
            _closeButton.onClick.RemoveAllListeners();
            _sellButton.onClick.RemoveAllListeners();
            _loadCarButton.onClick.RemoveAllListeners();
            foreach (var carUI in _carsUI)
            {
                carUI.BuyButton.onClick.RemoveAllListeners();
                carUI.SellButton.onClick.RemoveAllListeners();
            }
        }
    }
}