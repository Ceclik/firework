using UnityEngine;

namespace Instructions
{
    public class EnterTriggerAnimation : MonoBehaviour
    {
        [SerializeField] private float movingSpeed;

        [Header("Directions")] [SerializeField]
        private bool vertical;

        [SerializeField] private float higherPosition;
        [SerializeField] private float lowerPosition;

        [Space(10)] [SerializeField] private bool horizontal;
        [SerializeField] private float leftPosition;
        [SerializeField] private float rightPosition;
        private bool _isMovingDown = true;

        private bool _isMovingLeft = true;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            if (horizontal)
            {
                if (_isMovingLeft)
                    _rectTransform.Translate(new Vector2(-movingSpeed * Time.deltaTime, 0.0f));
                else
                    _rectTransform.Translate(new Vector2(movingSpeed * Time.deltaTime, 0.0f));

                if (_rectTransform.anchoredPosition.x < leftPosition)
                    _isMovingLeft = false;
                if (_rectTransform.anchoredPosition.x > rightPosition)
                    _isMovingLeft = true;
            }

            if (vertical)
            {
                if (_isMovingDown)
                    _rectTransform.Translate(new Vector2(0.0f, -movingSpeed * Time.deltaTime));
                else
                    _rectTransform.Translate(new Vector2(0.0f, movingSpeed * Time.deltaTime));

                if (_rectTransform.anchoredPosition.y < lowerPosition)
                    _isMovingDown = false;
                if (_rectTransform.anchoredPosition.y > higherPosition)
                    _isMovingDown = true;
            }
        }
    }
}