using UnityEngine;

namespace _Game.Scripts.Views.Abstractions
{
    public interface IBoardView
    {
        GameObject TileSlotPrefab { get; }
        Transform TileSlotParent { get; }
        int Width { get; }
        int Height { get; }
        float Offset { get; }
    }
}