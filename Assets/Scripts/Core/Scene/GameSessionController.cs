using System;
using System.Collections.Generic;
using CarsSystem;
using CarsSystem.Data;
using CarsSystem.Storages;
using Core.PlayerAccount;
using Core.Services.Updater;
using Core.Timer;
using Core.UI;
using Core.UI.WarehouseInventory;
using Map;
using PathBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using WarehousingSystem.Behaviour;
using WarehousingSystem.Controllers;
using WarehousingSystem.Data;
using WarehousingSystem.Storages;

namespace Core.Scene
{
    public class GameSessionController : MonoBehaviour
    {
        [Header("Game parameters")] 
        [SerializeField] private float _timeInMinutes;
        [SerializeField] private TextMeshProUGUI _currentTimeText;
        [SerializeField] private int _startingMoneyAmount;
        
        [Header("UI")] 
        [SerializeField] private GameSessionMenuUIPresenter _gameUIPresenter;
        
        [Header("Map Camera")]
        [SerializeField] private Camera _cam;
        [SerializeField] private float _zoomStep;
        [SerializeField] private float _minCamSize;
        [SerializeField] private TilemapRenderer _mapRenderer;
        
        [Header("Path Drawing")] 
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _pathPoints;

        [Header("Cars System")] 
        [SerializeField] private CarsStorage _carsStorage;
        
        [Header("Warehousing System")] 
        [SerializeField] private WarehouseInventoryUIController _warehouseInventoryController;
        [SerializeField] private WarehousesStorage _warehousesStorage;
        [SerializeField] private List<WarehouseScene> _warehousesBehaviours;

        private PlayerAccountController _playerAccount;
        private ProjectUpdater _projectUpdater;
        private TimerController _timerController;
        private MapCameraController _mapCameraController;
        private PathDrawer _pathDrawer;
        private CarSystem _carsSystem;
        private List<IDisposable> _disposables;
    
    
        private void Awake()
        {
            _disposables = new List<IDisposable>();

            if (ProjectUpdater.Instance == null)
            {
                _projectUpdater = new GameObject().AddComponent<ProjectUpdater>();
                _projectUpdater.gameObject.name = nameof(ProjectUpdater);
            }
            else
            {
                _projectUpdater = (ProjectUpdater)ProjectUpdater.Instance;
                _projectUpdater.IsPaused = false;
            }
            
            _playerAccount = new PlayerAccountController(_startingMoneyAmount);
            _gameUIPresenter.GameUIHidden += PauseGame;
            _gameUIPresenter.GameUIShown += UnpauseGame;
            _gameUIPresenter.Initialize(_playerAccount, _warehouseInventoryController);

            _timerController = new TimerController(_timeInMinutes, _currentTimeText);
            _disposables.Add(_timerController);
            _timerController.TimeExpired += FinishGame;

            List<CarDescriptor> carDescriptors = _carsStorage.CarDescriptors;
            CarsFactory carsFactory = new CarsFactory(carDescriptors);
            _carsSystem = new CarSystem(carsFactory);
            _disposables.Add(_carsSystem);
            
            _pathDrawer = new PathDrawer(_lineRenderer, _pathPoints, _carsSystem);
            _disposables.Add(_pathDrawer);
            
            _warehouseInventoryController.Initialize(carDescriptors, _pathDrawer);
            
            foreach (var warehouseBehaviour in _warehousesBehaviours)
            {
                WarehouseDescriptor descriptor = _warehousesStorage.WarehouseDescriptors.Find(descriptor => descriptor.Id == warehouseBehaviour.WarehouseId);

                WarehouseEntity warehouseEntity = new WarehouseEntity(descriptor, warehouseBehaviour, _warehouseInventoryController, carsFactory);
                _disposables.Add(warehouseEntity);
            }
        }

        private void Start()
        {
            _mapCameraController = new MapCameraController(_cam, _zoomStep, _minCamSize, _mapRenderer, _warehouseInventoryController);
            _disposables.Add(_mapCameraController);
        }
        
        private void OnDestroy()
        {
            _gameUIPresenter.GameUIHidden -= PauseGame;
            _gameUIPresenter.GameUIShown -= UnpauseGame;
            _timerController.TimeExpired -= FinishGame;
            
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private void PauseGame()
        {
            _projectUpdater.IsPaused = true;
        }

        private void UnpauseGame()
        {
            _projectUpdater.IsPaused = false;
        }

        private void FinishGame()
        {
            _gameUIPresenter.FinishGame();
        }
    }
}