using System;
using _Game.Scripts.Components;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Utilities;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Presenters
{
    public class BoardPresenter : IBoardPresenter
    {
        private ObjectPool<ITileSlotComponent> _tileSlotPool;
        private ObjectPool<ITileComponent> _tilePool;

        private IBoardView _boardView;
        private IBoard _board;
        
        public BoardPresenter(IBoardView boardView, IBoard board)
        {
            _boardView = boardView;
            _board = board;
        }
        
        public void Initialize()
        {
            SetupCamera();
            GenerateTileSlotPool();
            GenerateTileSlots();
            
            GenerateTilePool();
            FillTileSlots();
        }
        
        private void GenerateTileSlotPool()
        {
            _tileSlotPool = Pool.GeneratePool<ITileSlotComponent>(_boardView.TileSlotPrefab, _boardView.TileSlotParent, OnGetTileSlot, 
                OnReleaseTileSlot, OnResetTileSlot, 100);
        }
        
        private void GenerateTilePool()
        {
            _tilePool = Pool.GeneratePool<ITileComponent>(_boardView.TilePrefab, _boardView.TileParent, OnGetTile, 
                OnReleaseTile, OnResetTile, 100);
        }

        private void GenerateTileSlots()
        {
            _board.TileSlots = new ITileSlotComponent[_boardView.Width, _boardView.Height];
            
            for (int x = 0; x < _boardView.Width; x++)
            {
                for (int y = 0; y < _boardView.Height; y++)
                {
                    var tileSlot = _tileSlotPool.Get();
                    tileSlot.Transform.localPosition = new Vector3(x * _boardView.Offset, y * _boardView.Offset, 0);
                    _board.TileSlots[x, y] = tileSlot;
                }
            }
        }

        private void SetupCamera()
        {
            Camera.main.transform.position = new Vector3((_boardView.Width - 1f) / 2f, (_boardView.Height - 1f) / 2f, -10f);
            var aspectRatio = (float)Screen.width / Screen.height;
            var verticalSize = (float)_boardView.Height / 2f + _boardView.BoardSize;
            var horizontalSize = ((float)_boardView.Width / 2f + _boardView.BoardSize) / aspectRatio;
            Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
        }
        
        private void FillTileSlots()
        {
            for (int x = 0; x < _boardView.Width; x++)
            {
                for (int y = 0; y < _boardView.Height; y++)
                {
                    ITileComponent tile;
                    bool isSameColorAsNeighbours;
                    do
                    {
                        tile = _tilePool.Get();
                        tile.Initialize(GetRandomTileType());
                        
                        isSameColorAsNeighbours = IsSameColorAsNeighbours(x, y, tile.TileType);
                        if (isSameColorAsNeighbours)
                        {
                            tile.OnRelease();
                        }

                    } while (isSameColorAsNeighbours);

                    var tileSlot = _board.TileSlots[x, y];
                    tileSlot.TileComponent = tile;

                    tile.Transform.position = tileSlot.Transform.position;
                }
            }
        }

        private bool IsSameColorAsNeighbours(int x, int y, TileType tileType)
        {
            if (x > 1 && _board.TileSlots[x - 1, y].TileComponent != null && _board.TileSlots[x - 2, y].TileComponent != null)
            {
                if (_board.TileSlots[x - 1, y].TileComponent.TileType == tileType && _board.TileSlots[x - 2, y].TileComponent.TileType == tileType)
                {
                    return true;
                }
            }

            if (y > 1 && _board.TileSlots[x, y - 1].TileComponent != null && _board.TileSlots[x, y - 2].TileComponent != null)
            {
                if (_board.TileSlots[x, y - 1].TileComponent.TileType == tileType && _board.TileSlots[x, y - 2].TileComponent.TileType == tileType)
                {
                    return true;
                }
            }

            return false;
        }

        private TileType GetRandomTileType()
        {
            return (TileType)Random.Range(0, Enum.GetValues(typeof(TileType)).Length);
        }

        private void OnGetTileSlot<T>(T obj) where T : class
        {
            if (obj is ITileSlotComponent tileSlot)
            {
                tileSlot.OnGet();
            }
        }
        
        private void OnReleaseTileSlot<T>(T obj) where T : class
        {
            if (obj is ITileSlotComponent tileSlot)
            {
                tileSlot.OnRelease();
            }
        }

        private void OnResetTileSlot<T>(T obj) where T : class
        {
            if (obj is ITileSlotComponent tileSlot)
            {
                tileSlot.OnReset();
            }
        }
        
        private void OnGetTile<T>(T obj) where T : class
        {
            if (obj is ITileComponent tile)
            {
                tile.OnGet();
            }
        }
        
        private void OnReleaseTile<T>(T obj) where T : class
        {
            if (obj is ITileComponent tile)
            {
                tile.OnRelease();
            }
        }
        
        private void OnResetTile<T>(T obj) where T : class
        {
            if (obj is ITileComponent tile)
            {
                tile.OnReset();
            }
        }
    }
}