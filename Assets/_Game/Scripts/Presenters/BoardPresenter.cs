using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Utilities;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Presenters
{
    public class BoardPresenter : IBoardPresenter
    {
        private ObjectPool<ITileSlotComponent> _tileSlotPool;

        private IBoardView _boardView;
        private IBoard _board;
        
        public BoardPresenter(IBoardView boardView, IBoard board)
        {
            _boardView = boardView;
            _board = board;
        }
        
        public void Initialize()
        {
            GenerateTileSlotPool();
            GenerateTileSlots();
        }
        
        private void GenerateTileSlotPool()
        {
            _tileSlotPool = Pool.GeneratePool<ITileSlotComponent>(_boardView.TileSlotPrefab, _boardView.TileSlotParent, OnGetTileSlot, 
                OnReleaseTileSlot, OnResetTileSlot, 100);
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
    }
}