using UnityEngine;

namespace _Game.Scripts.Components.Abstractions
{
    public interface ITileSlotComponent
    {
        Transform Transform { get; }
        int XIndex { get; }
        int YIndex { get; }
        void Initialize(int xIndex, int yIndex);
        void OnGet();
        void OnRelease();
        void OnReset();
    }
}