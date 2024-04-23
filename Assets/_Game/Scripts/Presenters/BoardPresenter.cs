using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Game.Scripts.Components;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Utilities;
using _Game.Scripts.Views;
using _Game.Scripts.Views.Abstractions;
using Cysharp.Threading.Tasks;
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
        private bool isSwiping;
        
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
        
        public void SwipeTiles(ITilePresenter tile1)
        {
            if (isSwiping ||
                tile1.SwipeDirection == SwipeDirection.None ||
                tile1.SwipeDirection == SwipeDirection.Corner)
            {
                return;
            }
            
            SwipeTilesAsync(tile1).Forget();
        }
        
        private async UniTask SwipeTilesAsync(ITilePresenter tile1)
        {
            isSwiping = true;
            
            (int, int) targetIndexes = tile1.SwipeDirection switch
            {
                SwipeDirection.Left => (tile1.GetXIndex() - 1, tile1.GetYIndex()),
                SwipeDirection.Right => (tile1.GetXIndex() + 1, tile1.GetYIndex()),
                SwipeDirection.Up => (tile1.GetXIndex(), tile1.GetYIndex() + 1),
                SwipeDirection.Down => (tile1.GetXIndex(), tile1.GetYIndex() - 1),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (targetIndexes.Item1 < 0 || targetIndexes.Item1 >= _board.Width || 
                targetIndexes.Item2 < 0 || targetIndexes.Item2 >= _board.Height)
            {
                tile1.SwipeDirection = SwipeDirection.None;
                isSwiping = false;
                return;
            }
            
            var tile2 = _board.TileProps[targetIndexes.Item1, targetIndexes.Item2].TilePresenter;
            if (tile2 == null)
            {
                tile1.SwipeDirection = SwipeDirection.None;
                isSwiping = false;
                return;
            }
            
            await ReplaceTilesAsync(tile1, tile2);

            var tile1Matches = _board.FindMatchesAt(tile1.GetXIndex(), tile1.GetYIndex());
            var tile2Matches = _board.FindMatchesAt(tile2.GetXIndex(), tile2.GetYIndex());
            if (tile1Matches.Count == 0 && tile2Matches.Count == 0)
            {
                await UniTask.Delay(100);
                await ReplaceTilesAsync(tile1, tile2);
            }
            else
            {
                await UniTask.Delay(100);
                
                var allMatches = tile1Matches.Union(tile2Matches).ToList();
                await ClearAndReFillMatchesAsync(allMatches);
            }
            

            tile1.SwipeDirection = SwipeDirection.None;
            isSwiping = false;
        }
        
        private async UniTask RefillBoardAsync(int falseYOffset = 0, float moveTime = 0.1f)
        {
            int maxInterations = 100;
            int iterations = 0;

            var refillTaskList = new List<UniTask>();
            for (int i = 0; i < _board.Width; i++)
            {
                if (!_board.NonSpawnerRows.Contains(i))
                {
                    for (int j = 0; j < _board.Height; j++)
                    {
                        if (_board.TileProps[i, j].TilePresenter == null)
                        {
                            var piece = FillRandomAt(i, j, falseYOffset, moveTime);
                            refillTaskList.Add(piece);
                            iterations = 0;

                            while (_board.HasMatchOnFill(i, j))
                            {
                                _board.ClearPieceAt(i, j);
                                piece = FillRandomAt(i, j, falseYOffset, moveTime);
                                refillTaskList.Add(piece);
                                iterations++;

                                if (iterations >= maxInterations)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            
            await UniTask.WhenAll(refillTaskList);
        }
        
        private async UniTask<ITilePresenter> FillRandomAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
        {
            var tileView = _tilePool.Get();
            var tile = new Tile();
            var tilePresenter = new TilePresenter(tileView, tile);
            _board.TileProps[x, y] = new TileProp(tile, tilePresenter);
            var tileType = GetTileType(x, y, false);
            tilePresenter.Initialize(tileType.Item1, tileType.Item2, x, y);
            tilePresenter.OnSwipeAction += SwipeTiles;

            var tileSlot = _board.TileSlots[x, y];
            tileView.Transform.position = tileSlot.Transform.position + Vector3.up * falseYOffset;

            await tileView.MoveDownAsync(y, moveTime);
            return tilePresenter;
        }
        
        private async UniTask ClearAndReFillMatchesAsync(List<ITilePresenter> allMatches)
        {
            var matches = allMatches;

            do
            {
                await _board.ClearAndCollapseAsync(matches);
                await RefillBoardAsync(10, 0.5f);
                matches = _board.FindAllMatches();
                await UniTask.Delay(500);

            } while (matches.Count != 0);
        }

        private async UniTask ReplaceTilesAsync(ITilePresenter tile1, ITilePresenter tile2)
        {
            var task1 = tile1.SwipeToAsync(tile2.GetPosition());
            var task2 = tile2.SwipeToAsync(tile1.GetPosition());
            await UniTask.WhenAll(task1, task2);
            
            var tile1Prop = _board.TileProps[tile1.GetXIndex(), tile1.GetYIndex()];
            var tile2Prop = _board.TileProps[tile2.GetXIndex(), tile2.GetYIndex()];
            
            _board.TileProps[tile1.GetXIndex(), tile1.GetYIndex()] = tile2Prop;
            _board.TileProps[tile2.GetXIndex(), tile2.GetYIndex()] = tile1Prop;
            
            tile1Prop.TilePresenter = tile2;
            tile2Prop.TilePresenter = tile1;
            
            var tile1TempIndexes = (tile1.GetXIndex(), tile1.GetYIndex());
            tile1Prop.Tile.SetPosition(tile2.GetXIndex(), tile2.GetYIndex());
            tile2Prop.Tile.SetPosition(tile1TempIndexes.Item1, tile1TempIndexes.Item2);
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
            _board.TileSlots = new ITileSlotComponent[_board.Width, _board.Height];
            
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
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
            Camera.main.transform.position = new Vector3((_board.Width - 1f) / 2f, (_board.Height - 1f) / 2f, -10f);
            var aspectRatio = (float)Screen.width / Screen.height;
            var verticalSize = (float)_board.Height / 2f + _boardView.BoardSize;
            var horizontalSize = ((float)_board.Width / 2f + _boardView.BoardSize) / aspectRatio;
            Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
        }
        
        private void GenerateTiles()
        {
            _board.TileProps = new TileProp[_board.Width, _board.Height];
            
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var tileView = _tilePool.Get();
                    var tile = new Tile();
                    var tilePresenter = new TilePresenter(tileView, tile);
                    _board.TileProps[x, y] = new TileProp(tile, tilePresenter);
                    var tileType = GetTileType(x, y, true);
                    tilePresenter.Initialize(tileType.Item1, tileType.Item2, x, y);
                    tilePresenter.OnSwipeAction += SwipeTiles;

                    var tileSlot = _board.TileSlots[x, y];
                    tileView.Transform.position = tileSlot.Transform.position;
                }
            }
        }
        
        private (TileColor, string) GetTileType(int x, int y, bool isInitial)
        {
            var randomTileType = GetRandomTileType();

            if (isInitial && IsSameColorAsNeighbours(x, y, randomTileType))
            {
                return GetTileType(x, y, isInitial);
            }
            else
            {
                var spriteName = randomTileType switch
                {
                    TileColor.YELLOW => YellowSpriteName,
                    TileColor.RED => RedSpriteName,
                    TileColor.GREEN => GreenSpriteName,
                    TileColor.BLUE => BlueSpriteName,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                return (randomTileType, spriteName);
            }
        }

        private bool IsSameColorAsNeighbours(int x, int y, TileColor tileType)
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

        private TileColor GetRandomTileType()
        {
            return (TileColor)Random.Range(0, Enum.GetValues(typeof(TileColor)).Length - 1);
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