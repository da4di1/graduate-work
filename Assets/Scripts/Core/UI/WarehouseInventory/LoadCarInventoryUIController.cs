using System;
using System.Collections.Generic;
using System.Linq;
using CarsSystem.Enums;
using CarsSystem.Interfaces;
using PathBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WarehousingSystem.Controllers;
using CarDescriptor = CarsSystem.Data.CarDescriptor;

namespace Core.UI.WarehouseInventory
{
    public class LoadCarInventoryUIController : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _startCarButton;
        [SerializeField] private TextMeshProUGUI _currentProductsWeight;
        [SerializeField] private TextMeshProUGUI _maxWeight;
        [SerializeField] private TextMeshProUGUI _incomeFromProducts;
        [SerializeField] private List<CarUI> _carsUI;
        
        private List<CarDescriptor> _carsTypesDescriptors;
        private PathDrawer _pathDrawer;
        private int _productsTotalIncome;
        private int _productsTotalWeight;
        private Button _pickedCarButton;
        private TextMeshProUGUI _pickedCarAmountForLoading;
        private TextMeshProUGUI _pickedCarAmount;
        private ICarInformation _pickedCar;

        
        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Initialize(List<CarDescriptor> carsTypesDescriptors, PathDrawer pathDrawer)
        {
            _carsTypesDescriptors = carsTypesDescriptors;
            _pathDrawer = pathDrawer;
        }
        
        public void Show(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action startCarButtonClicked)
        {
            gameObject.SetActive(true);
            
            _incomeFromProducts.text = _productsTotalIncome.ToString();
            _currentProductsWeight.text = "0";
            
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carTypeDescriptor = _carsTypesDescriptors.FirstOrDefault(descriptor => descriptor.CarType == carUI.CarType);
                if (carTypeDescriptor == null) continue;
                carUI.Amount.text = warehouseEntity.Cars.Count(car => car.Descriptor.CarType == carUI.CarType).ToString();
                carUI.AmountForLoading.text = "-0";
                carUI.Capacity.text = carTypeDescriptor.Capacity + "kg";
                carUI.Icon.sprite = carTypeDescriptor.HorizontalSprite;
                
                carUI.PickButton.onClick.AddListener(() =>
                {
                    if (_pickedCarButton != null)
                    {
                        _pickedCarButton.interactable = true;
                        _pickedCarAmountForLoading.text = "-0";
                        warehouseEntity.Cars.Add(_pickedCar);
                        _pickedCarAmount.text = warehouseEntity.Cars.Count(car => car.Descriptor.CarType == carUI.CarType).ToString();
                    }
                    carUI.PickButton.interactable = false;
                    _pickedCarButton = carUI.PickButton;
                    carUI.AmountForLoading.text = "-1";
                    _pickedCarAmountForLoading = carUI.AmountForLoading;
                    _pickedCar = warehouseEntity.Cars.Find(car => car.Descriptor.CarType == carUI.CarType);
                    warehouseEntity.Cars.Remove(_pickedCar);
                    carUI.Amount.text = warehouseEntity.Cars.Count(car => car.Descriptor.CarType == carUI.CarType).ToString();
                    _pickedCarAmount = carUI.Amount;
                    _maxWeight.text = carUI.Capacity.text;
                });
            }
            
            _closeButton.onClick.AddListener(() =>
            {
                Hide();
                closeButtonClicked?.Invoke();
            });
            _startCarButton.onClick.AddListener(() =>
            {
                Hide();
                _pathDrawer.StartDrawingPath();
                startCarButtonClicked?.Invoke();
            });
            
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
            RemoveButtonsListeners();
        }

        private void RemoveButtonsListeners()
        {
            _closeButton.onClick.RemoveAllListeners();
            _startCarButton.onClick.RemoveAllListeners();
            foreach (var carUI in _carsUI)
            {
                carUI.PickButton.onClick.RemoveAllListeners();
            }
        }
        
        
        [Serializable]
        private class CarUI
        {
            [field: SerializeField] public CarType CarType { get; private set; }
            [field: SerializeField] public TextMeshProUGUI Amount { get; private set; }
            [field: SerializeField] public TextMeshProUGUI AmountForLoading { get; private set; }
            [field: SerializeField] public TextMeshProUGUI Capacity { get; private set; }
            [field: SerializeField] public Button PickButton { get; private set; }
            [field: SerializeField] public Image Icon { get; private set; }
        }
    }
}