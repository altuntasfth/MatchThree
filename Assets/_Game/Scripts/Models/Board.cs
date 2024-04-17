using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;

namespace _Game.Scripts.Models
{
    public class Board : IBoard
    {
        public ITileSlotComponent[,] TileSlots { get; set; }
        public ITilePresenter[,] TilePresenters { get; set; }
    }
}
