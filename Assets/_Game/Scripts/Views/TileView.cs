using System;
using _Game.Scripts.Models;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

namespace _Game.Scripts.Views
{
    public enum SwipeDirection
    {
        Right,
        Up,
        Left,
        Down,
        Corner
    }
    
    public class TileView : MonoBehaviour, ITileView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteAtlas spriteAtlas;
        
        [Header("Parameters"), Space(10)]
        [SerializeField] private float angleOffset = 30f;
        [SerializeField] private float swipeThreshold = 20f;
        
        private const float rightAngle = 0f;
        private const float upAngle = 90f;
        private const float leftAngle = 180f;
        private const float downAngle = 270f;
        
        private SwipeDirection _swipeDirection;
        private Vector2 _firstTouchPosition;
        private Vector2 _lastTouchPosition;
        private float _swipeAngle;
        private bool _isSwiping;
        
        public Transform Transform => gameObject.transform;
        
        public void Initialize(string spriteName)
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite(spriteName);
        }
        
        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }

        public void OnReset()
        {
            spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }

        private void CalculateSwipeAngle()
        {
            Vector2 swipeDirection = _lastTouchPosition - _firstTouchPosition;
            _swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;
            _swipeAngle = (_swipeAngle < 0) ? 360 + _swipeAngle : _swipeAngle;
            
            if (_swipeAngle > rightAngle - angleOffset && _swipeAngle < rightAngle + angleOffset)
            {
                _swipeDirection = SwipeDirection.Right;
            }
            else if (_swipeAngle > upAngle - angleOffset && _swipeAngle < upAngle + angleOffset)
            {
                _swipeDirection = SwipeDirection.Up;
            }
            else if (_swipeAngle > leftAngle - angleOffset && _swipeAngle < leftAngle + angleOffset)
            {
                _swipeDirection = SwipeDirection.Left;
            }
            else if (_swipeAngle > downAngle - angleOffset && _swipeAngle < downAngle + angleOffset)
            {
                _swipeDirection = SwipeDirection.Down;
            }
            else
            {
                _swipeDirection = SwipeDirection.Corner;
            }
            
            Debug.Log(_swipeDirection.ToString());
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _firstTouchPosition = eventData.position;
            _isSwiping = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isSwiping)
            {
                _lastTouchPosition = eventData.position;
                Vector2 swipeDirection = _lastTouchPosition - _firstTouchPosition;
                float swipeMagnitude = swipeDirection.magnitude;
                if (swipeMagnitude > swipeThreshold)
                {
                    CalculateSwipeAngle();
                    _isSwiping = false;
                    
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isSwiping = false;
        }
    }
}