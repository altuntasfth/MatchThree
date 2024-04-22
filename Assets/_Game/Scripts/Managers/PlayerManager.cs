using System;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        private IBoardPresenter _boardPresenter;
        private ITilePresenter _selectedTile;
        private ITilePresenter _targetTile;
        
        public float swipeThreshold = 100f;
        private Vector2 touchStartPos;
        private bool isSwiping = false; 
        
        public void Initialize(IBoardPresenter boardPresenter)
        {
            _boardPresenter = boardPresenter;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Moved:
                        if (isSwiping && Mathf.Abs(touch.position.x - touchStartPos.x) > swipeThreshold)
                        {
                            Vector2Int direction = GetSwipeDirection(touchStartPos, touch.position);
                            if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) == 1)
                            {
                                Vector2Int startGrid = GetGridPosition(touchStartPos);
                                if (IsMatch(startGrid, direction))
                                {
                                    SwapGrids(startGrid, startGrid + direction);
                                    isSwiping = false;
                                }
                                else
                                {
                                    isSwiping = false;
                                }
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                        isSwiping = false;
                        break;
                }
            }
        }
        
        private bool IsMatch(Vector2Int startGrid, Vector2Int direction)
        {
            if (direction.x != 0)
            {
                int consecutiveCount = 1;
                int x = startGrid.x + direction.x;
                int y = startGrid.y;

                while (x >= 0 && x < 8 && _boardPresenter.GetTile(new Vector2(x, y)).
                           IsTileType(_boardPresenter.GetTile(new Vector2(startGrid.x, startGrid.y)).GetTileType()))
                {
                    consecutiveCount++;
                    x += direction.x;
                }

                if (consecutiveCount >= 3)
                {
                    return true;
                }
            }
            else if (direction.y != 0)
            {
                int consecutiveCount = 1;
                int x = startGrid.x;
                int y = startGrid.y + direction.y;

                while (y >= 0 && y < 8 && _boardPresenter.GetTile(new Vector2(x, y)).
                           IsTileType(_boardPresenter.GetTile(new Vector2(startGrid.x, startGrid.y)).GetTileType()))
                {
                    consecutiveCount++;
                    y += direction.y;
                }

                if (consecutiveCount >= 3)
                {
                    return true;
                }
            }

            return false;
        }
        
        private Vector2Int GetGridPosition(Vector2 touchPosition)
        {
            int x = Mathf.FloorToInt(touchPosition.x / Screen.width * 8);
            int y = Mathf.FloorToInt(touchPosition.y / Screen.height * 8);

            x = Mathf.Clamp(x, 0, 7);
            y = Mathf.Clamp(y, 0, 7);

            return new Vector2Int(x, y);
        }
        
        private Vector2Int GetSwipeDirection(Vector2 touchStartPos, Vector2 touchEndPos)
        {
            Vector2 swipeDirection = touchEndPos - touchStartPos;

            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                return new Vector2Int((int)Mathf.Sign(swipeDirection.x), 0);
            }
            else
            {
                return new Vector2Int(0, (int)Mathf.Sign(swipeDirection.y));
            }
        }
        
        private void SwapGrids(Vector2Int startGrid, Vector2Int endGrid)
        {
            // GameObject tempGrid = _boardPresenter.GetTile(new Vector2(startGrid.x, startGrid.y));
            // grid[startGrid.x, startGrid.y] = grid[endGrid.x, endGrid.y];
            // grid[endGrid.x, endGrid.y] = tempGrid;
        }
    }
}