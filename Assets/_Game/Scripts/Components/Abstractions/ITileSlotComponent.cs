using UnityEngine;

namespace _Game.Scripts.Components.Abstractions
{
    public interface ITileSlotComponent
    {
        Transform Transform { get; }
        void OnGet();
        void OnRelease();
        void OnReset();
    }
}