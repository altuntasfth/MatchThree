using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;

namespace _Game.Scripts.Models
{
    public class Board : IBoard
    {
        public ITileSlotComponent[,] TileSlots { get; set; }
    }
}
