using System;
using System.Collections.Generic;
using System.Linq;
using CarsSystem.Data;
using CarsSystem.Enums;
using Core.UI.ModalUI;
using PathBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WarehousingSystem.Controllers;

namespace Core.UI.WarehouseInventory
{
    public class WarehouseInventoryUIController : MonoBehaviour, IWarehouseInventoryUIDisplayer, IWarehouseInventoryState
    {
        [SerializeField] private LoadCarInventoryUIController _loadCarInventory;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _loadCarButton;
        [SerializeField] private List<CarUI> _carsUI;
        
        private List<CarDescriptor> _carsTypesDescriptors;
        
        public bool IsWarehouseInventoryUIShown => gameObject.activeSelf || _loadCarInventory.gameObject.activeSelf;

        public event Action WarehouseInventoryAppeared;
        public event Action WarehouseInventoryDisappeared;
        

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Initialize(List<CarDescriptor> carsTypesDescriptors, PathDrawer pathDrawer)
        {
            _loadCarInventory.Initialize(carsTypesDescriptors, pathDrawer);
            _carsTypesDescriptors = carsTypesDescriptors;
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carTypeDescriptor = carsTypesDescriptors.FirstOrDefault(descriptor => descriptor.CarType == carUI.CarType);
                if (carTypeDescriptor == null) continue;
                carUI.Cost.text = carTypeDescriptor.Cost + "$";
                carUI.SalePrice.text = carTypeDescriptor.SalePrice + "$";
                carUI.Icon.sprite = carTypeDescriptor.HorizontalSprite;
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
                    _loadCarInventory.Show(warehouseEntity, SetActive, Hide);
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
                carUI.Amount.text = warehouseEntity.GetCarsAmount(carUI.CarType).ToString();
                
                CarDescriptor carTypeDescriptor = _carsTypesDescriptors.FirstOrDefault(descriptor => descriptor.CarType == carUI.CarType);
                if (carTypeDescriptor == null) continue;
                carUI.BuyButton.onClick.AddListener(() =>
                {
                    ModalUIController.Instance.Question.Show($"Do you want to buy this car for {carTypeDescriptor.Cost}?\n" +
                                                             $"Speed: {carTypeDescriptor.Speed} km/h\n" +
                                                             $"Capacity {carTypeDescriptor.Capacity} kg", () =>
                    {
                        warehouseEntity.BuyCar(carUI.CarType);
                        carUI.Amount.text = warehouseEntity.GetCarsAmount(carUI.CarType).ToString();
                    }, null);
                });
                carUI.SellButton.onClick.AddListener(() =>
                {
                    ModalUIController.Instance.Question.Show($"Do you want to sell this car for {carTypeDescriptor.SalePrice}?\n" +
                                                             $"Speed: {carTypeDescriptor.Speed} km/h\n" +
                                                             $"Capacity {carTypeDescriptor.Capacity} kg", () =>
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

        private void SetActive()
        {
            gameObject.SetActive(true);
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


        [Serializable]
        private class CarUI
        {
            [field: SerializeField] public CarType CarType { get; private set; }
            [field: SerializeField] public TextMeshProUGUI Amount { get; private set; }
            [field: SerializeField] public TextMeshProUGUI Cost { get; private set; }
            [field: SerializeField] public TextMeshProUGUI SalePrice { get; private set; }
            [field: SerializeField] public Button BuyButton { get; private set; }
            [field: SerializeField] public Button SellButton { get; private set; }
            [field: SerializeField] public Image Icon { get; private set; }
        }
    }
}