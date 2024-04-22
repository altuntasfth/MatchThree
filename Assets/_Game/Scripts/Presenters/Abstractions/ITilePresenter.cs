using System;
using _Game.Scripts.Models;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Views;
using _Game.Scripts.Views.Abstractions;

namespace _Game.Scripts.Presenters.Abstractions
{
    public interface ITilePresenter
    {
        bool IsTileType(TileType tileType);
        TileType GetTileType();
        void Initialize(TileType tileType, string spriteName, int xIndex, int yIndex);
    }
}