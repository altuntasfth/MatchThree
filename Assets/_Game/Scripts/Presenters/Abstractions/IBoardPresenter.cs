using _Game.Scripts.Views.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Presenters.Abstractions
{
    public interface IBoardPresenter
    {
        void Initialize();
        ITilePresenter GetTile(Vector2 eventDataPosition);
    }
}