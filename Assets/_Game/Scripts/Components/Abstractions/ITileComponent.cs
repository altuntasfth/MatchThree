using UnityEngine;

namespace _Game.Scripts.Components.Abstractions
{
    public interface ITileComponent
    {
        TileType TileType { get; set;  }
        Transform Transform { get; }
        void Initialize(TileType tileType);
        void OnGet();
        void OnRelease();
        void OnReset();
    }
}