using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.UI.ModalUI;
using Core.UI.WarehouseInventory.Data;
using PathBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WarehousingSystem.Controllers;
using CarDescriptor = CarsSystem.Data.CarDescriptor;

namespace Core.UI.WarehouseInventory.Controllers
{
    public class LoadCarInventoryUIController : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _startCarButton;
        [SerializeField] private TextMeshProUGUI _currentProductsWeight;
        [SerializeField] private TextMeshProUGUI _maxWeight;
        [SerializeField] private TextMeshProUGUI _incomeFromProducts;
        [SerializeField] private List<LoadCarInventoryCarUIDescriptor> _carsUI;
        [SerializeField] private Color _pickedColor;
        
        private int _productsTotalIncome;
        private int _productsTotalWeight;
        private PathDrawer _pathDrawer;
        private LoadCarInventoryCarUIDescriptor _currentlyPickedCarUI;
        private Color _standardCarUIColor;
        private List<CarDescriptor> _carsTypesDescriptors;
        
        
        private void OnDestroy()
        {
            RemoveButtonsListeners();
        }

        public void Initialize(List<CarDescriptor> carsTypesDescriptors, PathDrawer pathDrawer)
        {
            _carsTypesDescriptors = carsTypesDescriptors;
            _pathDrawer = pathDrawer;
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carTypeDescriptor = carsTypesDescriptors.FirstOrDefault(descriptor => descriptor.CarType == carUI.CarType);
                if (carTypeDescriptor == null) continue;
                carUI.Capacity.text = carTypeDescriptor.Capacity.ToString(CultureInfo.InvariantCulture);
                carUI.Icon.sprite = carTypeDescriptor.HorizontalSprite;
            }
        }
        
        public void Show(WarehouseEntity warehouseEntity, Action closeButtonClicked, Action startCarButtonClicked)
        {
            gameObject.SetActive(true);
            SetCarsUI(warehouseEntity);
            
            _incomeFromProducts.text = _productsTotalIncome.ToString();
            _currentProductsWeight.text = "0";
            _maxWeight.text = "0";
            
            _closeButton.onClick.AddListener(() =>
            {
                ModalUIController.Instance.Question.Show("Are you sure you want to stop the car loading process? " +
                                                         "\nYour actions will NOT be saved.", () =>
                {
                    Hide();
                    closeButtonClicked?.Invoke();
                }, null);
            });
            _startCarButton.onClick.AddListener(() =>
            {
                if (_currentlyPickedCarUI != null)
                {
                    ModalUIController.Instance.Question.Show("Are you sure you want to start the car with the selected products?", 
                        () =>
                    {
                        Hide();
                        _pathDrawer.StartDrawingPath();
                        startCarButtonClicked?.Invoke();
                    }, null);
                }
                else
                {
                    ModalUIController.Instance.Dialog.Show("You must pick at least one car for this process!", null);
                }
            });
        }

        private void SetCarsUI(WarehouseEntity warehouseEntity)
        {
            _currentlyPickedCarUI = null;
            foreach (var carUI in _carsUI)
            {
                CarDescriptor carTypeDescriptor = _carsTypesDescriptors.FirstOrDefault(descriptor => descriptor.CarType == carUI.CarType);
                if (carTypeDescriptor == null) continue;
                
                carUI.Amount.text = warehouseEntity.GetCarsAmount(carUI.CarType).ToString();
                carUI.PickButton.onClick.AddListener(() =>
                {
                    if (warehouseEntity.GetCarsAmount(carUI.CarType) > 0)
                    {
                        if (_currentlyPickedCarUI != null)
                        {
                            ChangeButtonColor(_currentlyPickedCarUI.PickButton, _standardCarUIColor);
                            _currentlyPickedCarUI.PickButton.interactable = true;
                        }
                        _currentlyPickedCarUI = carUI;
                        _standardCarUIColor = _currentlyPickedCarUI.PickButton.colors.disabledColor;
                        ChangeButtonColor(_currentlyPickedCarUI.PickButton, _pickedColor);
                        _currentlyPickedCarUI.PickButton.interactable = false;
                        _maxWeight.text = carUI.Capacity.text;
                    }
                    else
                    {
                        ModalUIController.Instance.Dialog.Show("You do not have any cars of this type!", null);
                    }
                });
            }
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
            if (_currentlyPickedCarUI != null)
            {
                ChangeButtonColor(_currentlyPickedCarUI.PickButton, _standardCarUIColor);
                _currentlyPickedCarUI.PickButton.interactable = true;
            }
            RemoveButtonsListeners();
        }
        
        private void ChangeButtonColor(Button button, Color color)
        {
            var buttonColors = button.colors;
            buttonColors.disabledColor = color;
            button.colors = buttonColors;
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
    }
}