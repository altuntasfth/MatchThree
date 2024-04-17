using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Presenters.Abstractions;

namespace _Game.Scripts.Models.Abstractions
{
    public interface IBoard
    {
        ITileSlotComponent[,] TileSlots { get; set; }
        ITilePresenter[,] TilePresenters { get; set; }
    }
}