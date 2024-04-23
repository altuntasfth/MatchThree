using System.Collections.Generic;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;
using Cysharp.Threading.Tasks;

namespace _Game.Scripts.Models.Abstractions
{
    public interface IBoard
    {
        ITileSlotComponent[,] TileSlots { get; set; }
        TileProp[,] TileProps { get; set; }
        int Width { get; }
        int Height { get; }
        List<ITilePresenter> FindAllMatches();
        List<ITilePresenter> FindMatchesAt(int x, int y, int minLength = 3);
        bool HasMatchOnFill(int x, int y, int minLength = 3);
        UniTask ClearPieceAt(List<ITilePresenter> gamePieces);
        UniTask ClearPieceAt(int x, int y);
        UniTask ClearAndCollapseAsync(List<ITilePresenter> gamePieces);
    }
}