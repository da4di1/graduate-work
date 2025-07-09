using UnityEngine;

namespace GameSession.Cars.Behaviour
{
    public class SceneCar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _carTransform;

        private Sprite _verticalSprite;
        private Sprite _horizontalSprite;
        
        public Vector2 Position => _carTransform.position;

        
        public void SetCar(Sprite verticalSprite, Sprite horizontalSprite, Vector2 position)
        {
            _verticalSprite = verticalSprite;
            _horizontalSprite = horizontalSprite;
            transform.position = position;
        }

        public void Move(Vector2 nextPosition, float speed)
        {
            Turn(nextPosition);
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
        }
        
        private void Turn(Vector2 nextPosition)
        {
            Vector2 direction = nextPosition - (Vector2)transform.position;
            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))  
            {
                _spriteRenderer.sprite = _verticalSprite;
                _spriteRenderer.flipY = direction.y > 0;
                _spriteRenderer.flipX = false;
            } 
            else 
            {
                _spriteRenderer.sprite = _horizontalSprite;
                _spriteRenderer.flipX = direction.x > 0;
                _spriteRenderer.flipY = false;
            }
        }
    }
}