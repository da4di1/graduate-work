using UnityEngine;

namespace CarsSystem.Behaviour
{
    public class SceneCar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Transform _carTransform;
        
        public Vector2 Position => _carTransform.position;

        public void SetCar(Sprite sprite, Vector2 position)
        {
            _sprite.sprite = sprite;
            transform.position = position;
        }

        public void Move(Vector2 nextPosition, float speed)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed);
            SetDirection(nextPosition, speed);
        }

        private void SetDirection(Vector2 nextPosition, float speed)
        {
            Vector2 direction = nextPosition - (Vector2)transform.position;
            float rotationAngle = Mathf.Atan2(direction.normalized.y, direction.normalized.x);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, rotationAngle * Mathf.Rad2Deg + 90f), speed);
        }
    }
}
