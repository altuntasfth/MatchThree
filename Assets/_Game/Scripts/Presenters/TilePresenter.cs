using _Game.Scripts.Models;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;

namespace _Game.Scripts.Presenters
{
    public class TilePresenter : ITilePresenter
    {
        private readonly ITileView _tileView;
        private ITile _tile;
        
        public TilePresenter(ITileView tileView)
        {
            _tileView = tileView;
        }
        
        public bool IsTileType(TileType tileType)
        {
            return _tile.TileType == tileType;
        }

        public void Initialize(TileType tileType, int xIndex, int yIndex)
        {
            _tile = new Tile();
            _tile.TileType = tileType;
            _tile.SetPosition(xIndex, yIndex);
            _tileView.Initialize(tileType);
        }
    }
}