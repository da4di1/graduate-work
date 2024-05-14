using System;
using System.Collections.Generic;
using CarsSystem;
using CarsSystem.Data;
using CarsSystem.Storages;
using Core.Services.Updater;
using PathBuilding;
using UnityEngine;
using WarehousingSystem.Behaviour;
using WarehousingSystem.Controllers;
using WarehousingSystem.Data;
using WarehousingSystem.Storages;

namespace Core.Scene
{
    public class GameLevelInitializer : MonoBehaviour
    {
        [Header("Path Drawing")] 
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("Cars System")] 
        [SerializeField] private CarsStorage _carsStorage;

        [Header("Warehousing System")] 
        [SerializeField] private WarehousesStorage _warehousesStorage;
        
        private ProjectUpdater _projectUpdater;
        private PathDrawer _pathDrawer;
        private CarSystem _carsSystem;
        private WarehouseScene[] _warehousesBehaviours;
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
                _projectUpdater = ProjectUpdater.Instance as ProjectUpdater;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _projectUpdater.IsPaused = !_projectUpdater.IsPaused;
            }
        }
    
        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
