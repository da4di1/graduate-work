using System;
using System.Collections.Generic;
using System.Linq;
using CarsSystem.Data;
using CarsSystem.Enums;
using Core.UI.DialogUI;
using Core.UI.QuestionUI;
using PathBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WarehousingSystem.Controllers;

namespace Core.UI.WarehouseInventory
{
    public class WarehouseInventoryController : MonoBehaviour, IWarehouseInventoryController
    {
        public static IWarehouseInventoryController Instance { get; private set; }

        [SerializeField] private LoadCarInventoryController _loadCarInventory;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _loadCarButton;
        [SerializeField] private List<CarUI> _carsUI;
        
        private WarehouseEntity _warehouseEntity;
        private List<CarDescriptor> _carsTypesDescriptors;
        private PathDrawer _pathDrawer;
        
        public bool IsWarehouseInventoryUIShown { get; private set; }

        public event Action WarehouseInventoryAppeared;
        public event Action WarehouseInventoryDisappeared;

        
        private void Awake()
        {
            Instance = this;
            
            Hide();
        }

        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Initialize(List<CarDescriptor> carsTypesDescriptors, PathDrawer pathDrawer)
        {
            _pathDrawer = pathDrawer;
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

        public void ShowWarehouseInventory(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action sellButtonClicked,
            Action loadCarButtonClicked)
        {
            WarehouseInventoryAppeared?.Invoke();
            gameObject.SetActive(true);
            IsWarehouseInventoryUIShown = true;
            _loadCarInventory.Closed += Hide;
            _warehouseEntity = warehouseEntity;
            SetCarsUI();
            
            _closeButton.onClick.AddListener(() =>
            {
                Hide();
                closeButtonClicked?.Invoke();
            });
            _sellButton.onClick.AddListener(() =>
            {
                QuestionUIController.Instance.ShowQuestion($"Do you want to sell this warehouse for {_warehouseEntity.Descriptor.SalePrice}?",
                    () =>
                    {
                        if (_warehouseEntity.GetCarsAmount() > 0)
                        {
                            DialogUIController.Instance.ShowDialog("Firstly you must sell all the cars in the warehouse!", null);
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
                if (_warehouseEntity.GetCarsAmount() > 0)
                {
                    loadCarButtonClicked?.Invoke();
                    _loadCarInventory.gameObject.SetActive(true);
                    _loadCarInventory.Initialize(_warehouseEntity, _carsTypesDescriptors, _pathDrawer);
                }
                else
                {
                    DialogUIController.Instance.ShowDialog("You must have at least one car for this process!", null);
                }
            });
        }

        private void SetCarsUI()
        {
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carDescriptor = _warehouseEntity.Cars.FirstOrDefault(car => car.Descriptor.CarType == carUI.CarType)?.Descriptor;
                carUI.Amount.text = carDescriptor == null ? "0" : _warehouseEntity.Cars.Count(car => car.Descriptor.CarType == carUI.CarType).ToString();
                
                CarDescriptor carTypeDescriptor = _carsTypesDescriptors.FirstOrDefault(descriptor => descriptor.CarType == carUI.CarType);
                if (carTypeDescriptor == null) continue;
                carUI.BuyButton.onClick.AddListener(() =>
                {
                    QuestionUIController.Instance.ShowQuestion($"Do you want to buy this car for {carTypeDescriptor.Cost}?\n" +
                                                               $"Speed: {carTypeDescriptor.Speed} km/h\n" +
                                                               $"Capacity {carTypeDescriptor.Capacity} kg", () =>
                    {
                        _warehouseEntity.BuyCar(carUI.CarType);
                        carUI.Amount.text = _warehouseEntity.Cars.Count(car => car.Descriptor.CarType == carUI.CarType).ToString();
                    }, null);
                });
                carUI.SellButton.onClick.AddListener(() =>
                {
                    QuestionUIController.Instance.ShowQuestion($"Do you want to sell this car for {carTypeDescriptor.SalePrice}?\n" +
                                                               $"Speed: {carTypeDescriptor.Speed} km/h\n" +
                                                               $"Capacity {carTypeDescriptor.Capacity} kg", () =>
                    {
                        _warehouseEntity.SellCar(carUI.CarType);
                        carUI.Amount.text = _warehouseEntity.Cars.Count(car => car.Descriptor.CarType == carUI.CarType).ToString();
                    }, null);
                });
            }
        }

        private void Hide()
        {
            IsWarehouseInventoryUIShown = false;
            WarehouseInventoryDisappeared?.Invoke();
            gameObject.SetActive(false);
            RemoveButtonsListeners();
        }

        private void RemoveButtonsListeners()
        {
            _loadCarInventory.Closed -= Hide;
            
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