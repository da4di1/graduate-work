using Core.ModalUI;
using Core.Services.Updater;
using GameSession.UI.WarehouseInventory.Interfaces;
using GameSession.Warehouses.Enums;
using UnityEngine;

namespace GameSession.Warehouses.Behaviour
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneWarehouse : MonoBehaviour
    {
        [SerializeField] private Color _forSaleColor;
        [SerializeField] private Color _ownedColor;
        [SerializeField] private SpriteRenderer _iconSprite;
        [SerializeField] private SpriteRenderer _backgroundSprite;
        [SerializeField] private float _backgroundTransparency;

        private bool _isBackgroundHidingDelayed;
        private IWarehouseInventoryState _warehouseInventoryState;
        
        [field: SerializeField] public WarehouseID WarehouseId { get; private set; }
        
        public bool Clicked { get; private set; }
        public bool Hovered { get; private set; }
        public bool Unhovered { get; private set; }


        private void OnMouseEnter()
        {
            Hovered = true;
        }

        private void OnMouseExit()
        {
            if (!_backgroundSprite.enabled) return;
            if (ProjectUpdater.Instance.IsPaused || ModalUIController.Instance.IsModalUIShown 
                                                 || _warehouseInventoryState.IsWarehouseInventoryUIShown)
            {
                if (_isBackgroundHidingDelayed) return;
                _isBackgroundHidingDelayed = true;
                ModalUIController.Instance.ModalUIDisappeared += TurnBackgroundOff;
                _warehouseInventoryState.WarehouseInventoryDisappeared += TurnBackgroundOff;
            }
            else
            {
                _backgroundSprite.enabled = false;
            }
        }

        private void OnMouseDown()
        {
            Clicked = true;
        }

        private void OnDestroy()
        {
            ModalUIController.Instance.ModalUIDisappeared -= TurnBackgroundOff;
            _warehouseInventoryState.WarehouseInventoryDisappeared -= TurnBackgroundOff;
        }

        public void Initialize(IWarehouseInventoryState warehouseInventoryState)
        {
            _warehouseInventoryState = warehouseInventoryState;
        }

        public void EnableBackground()
        {
            _backgroundSprite.enabled = true;
        }

        public void DisableBackground()
        {
            _backgroundSprite.enabled = false;
        }
        
        public void GetPurchased()
        {
            _iconSprite.color = _ownedColor;
            _backgroundSprite.color = _ownedColor;
            ChangeSpriteAlpha(_backgroundSprite, _backgroundTransparency);
        }

        public void GetSold()
        {
            _iconSprite.color = _forSaleColor;
            _backgroundSprite.color = _forSaleColor;
            ChangeSpriteAlpha(_backgroundSprite, _backgroundTransparency);
        }
        
        public void ResetOneTimeActions()
        {
            Clicked = false;
            Hovered = false;
            Unhovered = false;
        }

        private void TurnBackgroundOff()
        {
            if (ProjectUpdater.Instance.IsPaused || ModalUIController.Instance.IsModalUIShown 
                                                 || _warehouseInventoryState.IsWarehouseInventoryUIShown) return;
            _isBackgroundHidingDelayed = false;
            _backgroundSprite.enabled = false;
            ModalUIController.Instance.ModalUIDisappeared -= TurnBackgroundOff;
            _warehouseInventoryState.WarehouseInventoryDisappeared -= TurnBackgroundOff;
        }

        private void ChangeSpriteAlpha(SpriteRenderer sprite, float alpha)
        {
            var color = sprite.color;
            color.a = alpha;
            sprite.color = color;
        }
    }
}