using Core.Services.Updater;
using Core.UI;
using UnityEngine;
using WarehousingSystem.Enums;

namespace WarehousingSystem.Behaviour
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class WarehouseScene : MonoBehaviour
    {
        private const string GreenColor = "#52C54A";
        private const string BlueColor = "#284DE5";
        private const string RedColor = "#C02024";
        private const float TransparencyPercent = 0.15f;
        
        [SerializeField] private SpriteRenderer _iconSprite;
        [SerializeField] private SpriteRenderer _selectionBgSprite;
        
        [field: SerializeField] public WarehouseId WarehouseId { get; private set; }
        
        public bool Clicked { get; private set; }


        private void OnMouseEnter()
        {
            if (ProjectUpdater.Instance.IsPaused || QuestionUIController.Instance.IsQuestionUIShown) return;
            _selectionBgSprite.enabled = true;
        }

        private void OnMouseExit()
        {
            if (ProjectUpdater.Instance.IsPaused || QuestionUIController.Instance.IsQuestionUIShown)
            {
                QuestionUIController.Instance.QuestionDisappeared += TurnBackgroundOff;
            }
            else
            {
                _selectionBgSprite.enabled = false;
            }
        }

        private void OnMouseDown()
        {
            if (ProjectUpdater.Instance.IsPaused || QuestionUIController.Instance.IsQuestionUIShown) return;
            Clicked = true;
        }

        public void ResetOneTimeActions()
        {
            Clicked = false;
        }

        public void GetPurchased()
        {
            _iconSprite.sprite = Resources.Load<Sprite>($"{nameof(WarehousingSystem)}/{nameof(Sprite)}/YoursIcon");

            if (ColorUtility.TryParseHtmlString(BlueColor, out Color colorToSet))
            {
                _iconSprite.color = colorToSet;
                _selectionBgSprite.color = colorToSet;
                var color = _selectionBgSprite.color;
                color.a = TransparencyPercent;
                _selectionBgSprite.color = color;
            }
        }

        private void TurnBackgroundOff()
        {
            _selectionBgSprite.enabled = false;
            QuestionUIController.Instance.QuestionDisappeared -= TurnBackgroundOff;
        }
    }
}