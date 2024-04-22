using System;
using System.Collections.Generic;
using _Game.Scripts.Components;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models;
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
        private const string YellowSpriteName = "dropY";
        private const string RedSpriteName = "dropR";
        private const string GreenSpriteName = "dropG";
        private const string BlueSpriteName = "dropB";
        
        private ObjectPool<ITileSlotComponent> _tileSlotPool;
        private ObjectPool<ITileView> _tilePool;

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
            GenerateTiles();
        }

        public ITilePresenter GetTile(Vector2 eventDataPosition)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(eventDataPosition);
            var tileSlot = GetTileSlot(worldPosition);
            return _board.TileProps[tileSlot.XIndex, tileSlot.YIndex].TilePresenter;
        }

        private ITileSlotComponent GetTileSlot(Vector3 worldPosition)
        {
            var x = Mathf.RoundToInt(worldPosition.x);
            var y = Mathf.RoundToInt(worldPosition.y);
            return _board.TileSlots[x, y];
        }

        private void GenerateTileSlotPool()
        {
            _tileSlotPool = Pool.GeneratePool<ITileSlotComponent>(_boardView.TileSlotPrefab, _boardView.TileSlotParent, OnGetTileSlot, 
                OnReleaseTileSlot, OnResetTileSlot, 100);
        }
        
        private void GenerateTilePool()
        {
            _tilePool = Pool.GeneratePool<ITileView>(_boardView.TilePrefab, _boardView.TileParent, OnGetTile, 
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
                    tileSlot.Initialize(x, y);
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
        
        private void GenerateTiles()
        {
            _board.TileProps = new TileProp[_boardView.Width, _boardView.Height];
            
            for (int x = 0; x < _boardView.Width; x++)
            {
                for (int y = 0; y < _boardView.Height; y++)
                {
                    var tileView = _tilePool.Get();
                    var tile = new Tile();
                    var tilePresenter = new TilePresenter(tileView, tile);
                    _board.TileProps[x, y] = new TileProp(tile, tilePresenter);
                    var tileType = GetTileType(x, y);
                    tilePresenter.Initialize(tileType.Item1, tileType.Item2, x, y);

                    var tileSlot = _board.TileSlots[x, y];
                    tileView.Transform.position = tileSlot.Transform.position;
                }
            }
        }
        
        private (TileType, string) GetTileType(int x, int y)
        {
            var randomTileType = GetRandomTileType();

            if (IsSameColorAsNeighbours(x, y, randomTileType))
            {
                return GetTileType(x, y);
            }
            else
            {
                var spriteName = randomTileType switch
                {
                    TileType.YELLOW => YellowSpriteName,
                    TileType.RED => RedSpriteName,
                    TileType.GREEN => GreenSpriteName,
                    TileType.BLUE => BlueSpriteName,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                return (randomTileType, spriteName);
            }
        }

        private bool IsSameColorAsNeighbours(int x, int y, TileType tileType)
        {
            if (x > 1 && !_board.TileProps[x - 1, y].IsNull() && !_board.TileProps[x - 2, y].IsNull())
            {
                if (_board.TileProps[x - 1, y].TilePresenter.IsTileType(tileType) && 
                    _board.TileProps[x - 2, y].TilePresenter.IsTileType(tileType))
                {
                    return true;
                }
            }

            if (y > 1 && !_board.TileProps[x, y - 1].IsNull() && !_board.TileProps[x, y - 2].IsNull())
            {
                if (_board.TileProps[x, y - 1].TilePresenter.IsTileType(tileType) && 
                    _board.TileProps[x, y - 2].TilePresenter.IsTileType(tileType))
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
            if (obj is ITileView tile)
            {
                tile.OnGet();
            }
        }
        
        private void OnReleaseTile<T>(T obj) where T : class
        {
            if (obj is ITileView tile)
            {
                tile.OnRelease();
            }
        }
        
        private void OnResetTile<T>(T obj) where T : class
        {
            if (obj is ITileView tile)
            {
                tile.OnReset();
            }
        }
    }
}