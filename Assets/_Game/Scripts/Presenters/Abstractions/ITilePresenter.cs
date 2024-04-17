using _Game.Scripts.Models;

namespace _Game.Scripts.Presenters.Abstractions
{
    public interface ITilePresenter
    {
        bool IsTileType(TileType tileType);
        void Initialize(TileType tileType, int xIndex, int yIndex);
    }
}