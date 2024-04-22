using System;
using _Game.Scripts.Models;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views;
using _Game.Scripts.Views.Abstractions;

namespace _Game.Scripts.Presenters
{
    public class TilePresenter : ITilePresenter
    {
        public event Action<SwipeDirection, ITileView> OnSwipe;

        private readonly ITileView _tileView;
        private ITile _tile;
        
        public TilePresenter(ITileView tileView, ITile tile)
        {
            _tileView = tileView;
            _tile = tile;
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
    }
}