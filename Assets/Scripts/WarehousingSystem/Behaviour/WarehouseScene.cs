using Core.Services.Updater;
using Core.UI.ModalUI;
using Core.UI.WarehouseInventory;
using UnityEngine;
using WarehousingSystem.Enums;

namespace WarehousingSystem.Behaviour
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class WarehouseScene : MonoBehaviour
    {
        [SerializeField] private Color _forSaleColor;
        [SerializeField] private Color _ownedColor;
        [SerializeField] private SpriteRenderer _iconSprite;
        [SerializeField] private SpriteRenderer _backgroundSprite;
        [SerializeField] private float _backgroundTransparency;
        
        [field: SerializeField] public WarehouseId WarehouseId { get; private set; }
        
        public bool Clicked { get; private set; }


        private void OnMouseEnter()
        {
            if (ProjectUpdater.Instance.IsPaused || ModalUIController.Instance.IsModalUIShown ||
                WarehouseInventoryController.Instance.IsWarehouseInventoryUIShown) return;
            _backgroundSprite.enabled = true;
        }

        private void OnMouseExit()
        {
            if (ProjectUpdater.Instance.IsPaused || ModalUIController.Instance.IsModalUIShown || 
                WarehouseInventoryController.Instance.IsWarehouseInventoryUIShown)
            {
                Debug.Log("1");
                ModalUIController.Instance.ModalUIDisappeared += TurnBackgroundOff;
                WarehouseInventoryController.Instance.WarehouseInventoryDisappeared += TurnBackgroundOff;
            }
            else
            {
                _backgroundSprite.enabled = false;
            }
        }

        private void OnMouseDown()
        {
            if (ProjectUpdater.Instance.IsPaused || ModalUIController.Instance.IsModalUIShown || 
                WarehouseInventoryController.Instance.IsWarehouseInventoryUIShown) return;
            Clicked = true;
        }

        public void ResetOneTimeActions()
        {
            Clicked = false;
        }

        public void GetPurchased()
        {
            _iconSprite.color = _ownedColor;
            _backgroundSprite.color = _ownedColor;
            var backgroundColor = _backgroundSprite.color;
            backgroundColor.a = _backgroundTransparency;
            _backgroundSprite.color = backgroundColor;
        }

        public void GetSold()
        {
            _iconSprite.color = _forSaleColor;
            _backgroundSprite.color = _forSaleColor;
            var backgroundColor = _backgroundSprite.color;
            backgroundColor.a = _backgroundTransparency;
            _backgroundSprite.color = backgroundColor;
        }

        private void TurnBackgroundOff()
        {
            _backgroundSprite.enabled = false;
            Debug.Log("2");
            ModalUIController.Instance.ModalUIDisappeared -= TurnBackgroundOff;
            WarehouseInventoryController.Instance.WarehouseInventoryDisappeared -= TurnBackgroundOff;
        }
    }
}