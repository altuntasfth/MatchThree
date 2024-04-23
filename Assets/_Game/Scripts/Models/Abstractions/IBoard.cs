using System.Collections.Generic;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;

namespace _Game.Scripts.Models.Abstractions
{
    public interface IBoard
    {
        ITileSlotComponent[,] TileSlots { get; set; }
        TileProp[,] TileProps { get; set; }
        int Width { get; }
        int Height { get; }
        List<ITilePresenter> FindMatchesAt(int x, int y, int minLength = 3);
    }
}