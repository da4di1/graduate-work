using System;
using System.Collections.Generic;
using CarsSystem;
using CarsSystem.Data;
using CarsSystem.Storages;
using Core.Services.Updater;
using PathBuilding;
using UnityEngine;

namespace Core.Scene
{
    public class GameLevelInitializer : MonoBehaviour
    {
        [Header("Path Drawing")] 
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("Path Drawing")] 
        [SerializeField] private CarsStorage _carsStorage;
        
        private ProjectUpdater _projectUpdater;
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
                _projectUpdater = ProjectUpdater.Instance as ProjectUpdater;

            List<CarDescriptor> carDescriptors = _carsStorage.CarDescriptors;
            CarsFactory carsFactory = new CarsFactory(carDescriptors);
            _carsSystem = new CarSystem(carsFactory);
            _disposables.Add(_carsSystem);
            
            _pathDrawer = new PathDrawer(_lineRenderer, _carsSystem);
            _disposables.Add(_pathDrawer);
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
