using UnityEngine;

namespace CustomizableCharacters.CharacterEditor.Demo
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _arrowSprite;
        [SerializeField] private float _speed;
        private Vector2 _direction;
        private bool _isTraveling;
        private const float LifeTime = 1;
        private float _timer;

        public void StartMoving(Vector2 direction)
        {
            _direction = direction;
            _arrowSprite.transform.up = _direction;
            _isTraveling = true;
        }

        private void Update()
        {
            if (!_isTraveling)
                return;

            Move();

            _timer += Time.deltaTime;
            if (_timer > LifeTime)
                Destroy(gameObject);
        }

        private void Move()
        {
            var position = transform.position;
            var velocity = _direction * (_speed * Time.deltaTime);
            position += (Vector3)velocity;
            transform.position = position;
        }

        public void SetHeight(float height)
        {
            var localPosition = _arrowSprite.transform.localPosition;
            localPosition.y = height;
            _arrowSprite.transform.localPosition = localPosition;
        }
    }
}