using UnityEngine;

namespace _Game.Scripts.Components.Abstractions
{
    public interface ITileSlotComponent
    {
        Transform Transform { get; }
        ITileComponent TileComponent { get; set; }
        void OnGet();
        void OnRelease();
        void OnReset();
    }
}