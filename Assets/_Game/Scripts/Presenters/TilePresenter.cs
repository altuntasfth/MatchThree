using System;
using System.Threading;
using _Game.Scripts.Models;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views;
using _Game.Scripts.Views.Abstractions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Presenters
{
    public class TilePresenter : ITilePresenter, IDisposable
    {
        public Action<ITilePresenter> OnSwipeAction { get; set; }
        public SwipeDirection SwipeDirection
        {
            get => _swipeDirection;
            set => _swipeDirection = value;
        }

        public int GetXIndex()
        {
            return _tile.XIndex;
        }

        public int GetYIndex()
        {
            return _tile.YIndex;
        }

        private readonly ITileView _tileView;
        private ITile _tile;
        
        private SwipeDirection _swipeDirection;
        private Vector2 _firstTouchPosition;
        private Vector2 _lastTouchPosition;
        private float _swipeAngle;
        
        private const float rightAngle = 0f;
        private const float upAngle = 90f;
        private const float leftAngle = 180f;
        private const float downAngle = 270f;
        private bool _isDragging;
        
        public TilePresenter(ITileView tileView, ITile tile)
        {
            _tileView = tileView;
            _tile = tile;
            
            _tileView.OnBeginDragAction += OnBeginDrag;
            _tileView.OnDragAction += OnDrag;
            _tileView.OnEndDragAction += OnEndDrag;
        }
        
        public bool IsTileType(TileType tileType)
        {
            return _tile.TileType == tileType;
        }

        public TileType GetTileType()
        {
            return _tile.TileType;
        }

        public void Initialize(TileType tileType, string spriteName, int xIndex, int yIndex)
        {
            _tile.TileType = tileType;
            _tile.SetPosition(xIndex, yIndex);
            _tileView.Initialize(spriteName);
        }

        public Vector3 GetPosition()
        {
            return _tile.GetPosition();
        }

        public async UniTask SwipeToAsync(Vector3 targetPosition, CancellationToken cancellationToken = default)
        {
            _tile.IsMoving = true;
            await _tileView.SwipeToAsync(targetPosition, cancellationToken);
            _tile.IsMoving = false;
        }
        
        public async UniTask ScaleDownAsync(CancellationToken cancellationToken = default)
        {
            await _tileView.ScaleDownAsync(cancellationToken);
        }

        private void CalculateSwipeAngle()
        {
            Vector2 swipeDirection = _lastTouchPosition - _firstTouchPosition;
            _swipeAngle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;
            _swipeAngle = (_swipeAngle < 0) ? 360 + _swipeAngle : _swipeAngle;
            
            if (_swipeAngle > rightAngle - _tileView.AngleOffset && _swipeAngle < rightAngle + _tileView.AngleOffset)
            {
                _swipeDirection = SwipeDirection.Right;
            }
            else if (_swipeAngle > upAngle - _tileView.AngleOffset && _swipeAngle < upAngle + _tileView.AngleOffset)
            {
                _swipeDirection = SwipeDirection.Up;
            }
            else if (_swipeAngle > leftAngle - _tileView.AngleOffset && _swipeAngle < leftAngle + _tileView.AngleOffset)
            {
                _swipeDirection = SwipeDirection.Left;
            }
            else if (_swipeAngle > downAngle - _tileView.AngleOffset && _swipeAngle < downAngle + _tileView.AngleOffset)
            {
                _swipeDirection = SwipeDirection.Down;
            }
            else
            {
                _swipeDirection = SwipeDirection.Corner;
            }
            
            Debug.Log(_swipeDirection.ToString());
        }


        private void OnBeginDrag(PointerEventData eventData)
        {
            if (!_tile.IsMoving && !_isDragging)
            {
                _firstTouchPosition = eventData.position;
                _isDragging = true;
            }
        }

        private void OnDrag(PointerEventData eventData)
        {
            if (!_tile.IsMoving && _isDragging)
            {
                _lastTouchPosition = eventData.position;
                Vector2 swipeDirection = _lastTouchPosition - _firstTouchPosition;
                float swipeMagnitude = swipeDirection.magnitude;
                if (swipeMagnitude > _tileView.SwipeThreshold)
                {
                    CalculateSwipeAngle();
                    OnSwipeAction?.Invoke(this);
                    _isDragging = false;
                }
            }
        }

        private void OnEndDrag(PointerEventData eventData)
        {
        }


        public void Dispose()
        {
            _tileView.OnBeginDragAction -= OnBeginDrag;
            _tileView.OnDragAction -= OnDrag;
            _tileView.OnEndDragAction -= OnEndDrag;
            OnSwipeAction = null;
        }
    }
}