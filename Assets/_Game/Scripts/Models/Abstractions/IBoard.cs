using _Game.Scripts.Components.Abstractions;

namespace _Game.Scripts.Models.Abstractions
{
    public interface IBoard
    {
        ITileSlotComponent[,] TileSlots { get; set; }
    }
}