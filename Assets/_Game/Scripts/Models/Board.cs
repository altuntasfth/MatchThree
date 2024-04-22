using System.Collections.Generic;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;

namespace _Game.Scripts.Models
{
    public struct TileProp
    {
        public ITile Tile { get; set; }
        public ITilePresenter TilePresenter { get; set; }
        
        public TileProp(ITile tile, ITilePresenter tilePresenter)
        {
            Tile = tile;
            TilePresenter = tilePresenter;
        }
        
        public bool IsNull()
        {
            return Tile == null || TilePresenter == null;
        }
    }
    
    public class Board : IBoard
    {
        public ITileSlotComponent[,] TileSlots { get; set; }
        public TileProp[,] TileProps { get; set; }
    }
}
