using System;
using System.Collections.Generic;
using CarsSystem;
using CarsSystem.Data;
using CarsSystem.Storages;
using Core.Services.PlayFab;
using Core.Services.Updater;
using Core.Timer;
using Core.UI;
using Map;
using PathBuilding;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using WarehousingSystem.Behaviour;
using WarehousingSystem.Controllers;
using WarehousingSystem.Data;
using WarehousingSystem.Storages;

namespace Core.Scene
{
    public class GameController : MonoBehaviour
    {
        [Header("Timer")] 
        [SerializeField] private int _timeInMinutes;
        [SerializeField] private TextMeshProUGUI _currentTimeText;
        
        [Header("UI")] 
        [SerializeField] private Transform _userInterface;
        [SerializeField] private Transform _loadingScreen;
        [SerializeField] private LayerMask _viewUIMask;
        
        [Header("Map Camera")]
        [SerializeField] private Camera _cam;
        [SerializeField] private float _zoomStep;
        [SerializeField] private float _minCamSize;
        [SerializeField] private TilemapRenderer _mapRenderer;
        
        [Header("Path Drawing")] 
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("Cars System")] 
        [SerializeField] private CarsStorage _carsStorage;

        [Header("Warehousing System")] 
        [SerializeField] private WarehousesStorage _warehousesStorage;
        
        [Header("Post-Process")]
        [SerializeField] private PostProcessVolume _postProcessVolume;
        [SerializeField] private PostProcessLayer _postProcessLayer;
        
        private ProjectUpdater _projectUpdater;
        private TimerController _timerController;
        private MapCameraController _mapCameraController;
        private PathDrawer _pathDrawer;
        private CarSystem _carsSystem;
        private WarehouseScene[] _warehousesBehaviours;
        private Button[] _buttons;
        private List<Transform> _openedUIWindows;
        private List<WarehouseEntity> _warehouseEntities;
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
                _projectUpdater = ProjectUpdater.Instance as ProjectUpdater;
                if (_projectUpdater != null) _projectUpdater.IsPaused = false;
            }

            _openedUIWindows = new List<Transform>();

            _timerController = new TimerController(_timeInMinutes, _currentTimeText);
            _disposables.Add(_timerController);
            _timerController.TimeExpired += FinishGame;

            List<CarDescriptor> carDescriptors = _carsStorage.CarDescriptors;
            CarsFactory carsFactory = new CarsFactory(carDescriptors);
            _carsSystem = new CarSystem(carsFactory);
            _disposables.Add(_carsSystem);
            
            _pathDrawer = new PathDrawer(_lineRenderer, _carsSystem);
            _disposables.Add(_pathDrawer);

            _warehouseEntities = new List<WarehouseEntity>();
            _warehousesBehaviours = FindObjectsOfType<WarehouseScene>();
            foreach (var warehouseBehaviour in _warehousesBehaviours)
            {
                WarehouseDescriptor descriptor = _warehousesStorage.WarehouseDescriptors.Find(descriptor => descriptor.Id == warehouseBehaviour.WarehouseId);

                WarehouseEntity warehouseEntity = new WarehouseEntity(descriptor, warehouseBehaviour, _pathDrawer);
                _warehouseEntities.Add(warehouseEntity);
                _disposables.Add(warehouseEntity);
            }
        }

        private void Start()
        {
            _mapCameraController = new MapCameraController(_cam, _zoomStep, _minCamSize, _mapRenderer);
            _disposables.Add(_mapCameraController);

            QuestionUIController.Instance.QuestionAppeared += PauseInterface;
            QuestionUIController.Instance.QuestionDisappeared += UnPauseInterface;
        }
        
        private void OnDestroy()
        {
            _timerController.TimeExpired -= FinishGame;
            QuestionUIController.Instance.QuestionAppeared -= PauseInterface;
            QuestionUIController.Instance.QuestionDisappeared -= UnPauseInterface;
            
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        public void PauseGame()
        {
            _projectUpdater.IsPaused = true;
            
            foreach (Transform windowUI in _userInterface)
            {
                if (!windowUI.gameObject.activeSelf) continue;
                _openedUIWindows.Add(windowUI);
                windowUI.gameObject.SetActive(false);
            }
        }

        public void UnPauseGame()
        {
            _projectUpdater.IsPaused = false;
            
            foreach (var windowUI in _openedUIWindows)
            {
                windowUI.gameObject.SetActive(true);
            }
            _openedUIWindows.Clear();
        }

        private void PauseInterface()
        {
            _buttons = FindObjectsOfType<Button>();
            foreach (var button in _buttons)
            {
                if ((_viewUIMask.value & (1 << button.gameObject.layer)) != 0) continue;
                button.interactable = false;
            }
        }

        private void UnPauseInterface()
        {
            foreach (var button in _buttons)
            {
                if ((_viewUIMask.value & (1 << button.gameObject.layer)) != 0) continue;
                button.interactable = true;
            }
        }

        private void FinishGame()
        {
            PauseGame();
            _loadingScreen.gameObject.SetActive(true);
            _postProcessVolume.enabled = true;
            _postProcessLayer.enabled = true;
            PlayFabManager.Instance.UpdateLeaderboard(0);
        }
    }
}
