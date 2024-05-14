using System;
using Core.Services.Updater;
using Core.UI;
using PathBuilding;
using WarehousingSystem.Behaviour;
using WarehousingSystem.Data;

namespace WarehousingSystem.Controllers
{
    public class WarehouseEntity : IDisposable
    {
        private readonly WarehouseScene _warehouseBehaviour;
        private readonly WarehouseDescriptor _descriptor;
        private readonly PathDrawer _pathDrawer;
        private bool _isPurchasable;


        public WarehouseEntity(WarehouseDescriptor descriptor, WarehouseScene warehouseBehaviour, PathDrawer pathDrawer)
        {
            _descriptor = descriptor;
            _warehouseBehaviour = warehouseBehaviour;
            _isPurchasable = true;
            _pathDrawer = pathDrawer;

            ProjectUpdater.Instance.FixedUpdateCalled += OnFixedUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.FixedUpdateCalled -= OnFixedUpdate;
        }

        private void OnFixedUpdate()
        {
            if (_warehouseBehaviour.Clicked)
            {
                if (_isPurchasable)
                {
                    QuestionUIController.Instance.ShowQuestion($"Do you want to buy this warehouse for {_descriptor.Price}$?", () =>
                    {
                        _isPurchasable = false;
                        _warehouseBehaviour.GetPurchased();
                    }, (() => {}));
                }
                else
                {
                    _pathDrawer.StartDrawingPath();
                }
            }
            
            _warehouseBehaviour.ResetOneTimeActions();
        }
    }
}