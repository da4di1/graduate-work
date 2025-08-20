using System;
using System.Collections.Generic;
using System.Linq;
using Core.Services.Updater;
using GameSession.Cars;
using GameSession.Cars.Data;
using GameSession.Cars.Storages;
using GameSession.InputReader;
using GameSession.Map;
using GameSession.PathBuilding;
using GameSession.PlayerAccount;
using GameSession.Timer;
using GameSession.UI;
using GameSession.UI.WarehouseInventory.Controllers;
using GameSession.Warehouses;
using GameSession.Warehouses.Behaviour;
using GameSession.Warehouses.Controllers;
using GameSession.Warehouses.Data;
using GameSession.Warehouses.Storages;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSession.Scene
{
    public class GameSessionInitializer : MonoBehaviour
    {
        [Header("Game parameters")] 
        [SerializeField] private float _timeInMinutes;
        [SerializeField] private TextMeshProUGUI _currentTimeText;
        [SerializeField] private int _startingMoneyAmount;
        
        [Header("UI")] 
        [SerializeField] private GameSessionUIPresenter _gameUIPresenter;
        [SerializeField] private WarehouseInventoryUIController _warehouseInventoryController;
        
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
        [SerializeField] private WarehousesStorage _warehousesStorage;
        [SerializeField] private Transform _sceneWarehouses;
        /*[SerializeField] private List<SceneWarehouse> _warehousesBehaviours;*/

        private ProjectUpdater _projectUpdater;
        private PlayerAccountController _playerAccount;
        private MouseInputReader _mouseInputReader;
        private TouchInputReader _touchInputReader;
        private TimerController _timerController;
        private CarsSystem _carsSystem;
        private MapCameraController _mapCameraController;
        private PathDrawer _pathDrawer;
        private WarehousesSystem _warehousesSystem;
        private List<ISceneInputSource> _inputSources;
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
            _gameUIPresenter.GameUIHidden += PauseGameSession;
            _gameUIPresenter.GameUIShown += UnpauseGameSession;
            _gameUIPresenter.Initialize(_playerAccount);
            
            _mouseInputReader = new MouseInputReader();
            _disposables.Add(_mouseInputReader);
            _touchInputReader = new TouchInputReader();
            _disposables.Add(_touchInputReader);

            _inputSources = new List<ISceneInputSource>
            {
                _mouseInputReader,
                _touchInputReader,
            };

            _timerController = new TimerController(_timeInMinutes, _currentTimeText);
            _disposables.Add(_timerController);
            _timerController.TimeExpired += FinishGameSession;

            List<CarDescriptor> carDescriptors = _carsStorage.CarDescriptors;
            CarsFactory carsFactory = new CarsFactory(carDescriptors);
            _carsSystem = new CarsSystem(carsFactory);
            _disposables.Add(_carsSystem);
            
            _mapCameraController = new MapCameraController(_cam, _zoomStep, _minCamSize, _mapRenderer, _inputSources);
            _disposables.Add(_mapCameraController);
            
            List<PathPointDescriptor> pathPoints = _pathPoints.GetComponentsInChildren<PathPointDescriptor>(true).ToList();
            _pathDrawer = new PathDrawer(pathPoints, _lineRenderer, _mapCameraController, _carsSystem, _inputSources);
            _disposables.Add(_pathDrawer);
            
            _warehouseInventoryController.Initialize(carDescriptors, _pathDrawer);

            List<SceneWarehouse> sceneWarehouses = _sceneWarehouses.GetComponentsInChildren<SceneWarehouse>().ToList();
            List<WarehouseDescriptor> warehousesDescriptors = _warehousesStorage.WarehouseDescriptors;
            _warehousesSystem = new WarehousesSystem(sceneWarehouses, warehousesDescriptors, _warehouseInventoryController, _playerAccount,
                carsFactory);
            _disposables.Add(_warehousesSystem);
            
            /*foreach (var warehouseBehaviour in _warehousesBehaviours)
            {
                WarehouseDescriptor descriptor = _warehousesStorage.WarehouseDescriptors.Find(descriptor => descriptor.Id == warehouseBehaviour.WarehouseId);

                WarehouseEntity warehouseEntity = new WarehouseEntity(descriptor, warehouseBehaviour, _warehouseInventoryController, carsFactory);
                _disposables.Add(warehouseEntity);
            }*/
        }
        
        private void OnDestroy()
        {
            _gameUIPresenter.GameUIHidden -= PauseGameSession;
            _gameUIPresenter.GameUIShown -= UnpauseGameSession;
            _timerController.TimeExpired -= FinishGameSession;
            
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private void PauseGameSession()
        {
            _projectUpdater.IsPaused = true;
        }

        private void UnpauseGameSession()
        {
            _projectUpdater.IsPaused = false;
        }

        private void FinishGameSession()
        {
            _gameUIPresenter.FinishGameSession();
        }
    }
}