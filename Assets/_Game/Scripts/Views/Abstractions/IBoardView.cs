using UnityEngine;

namespace _Game.Scripts.Views.Abstractions
{
    public interface IBoardView
    {
        GameObject TileSlotPrefab { get; }
        Transform TileSlotParent { get; }
        GameObject TilePrefab { get; }
        Transform TileParent { get; }
        int Width { get; }
        int Height { get; }
        float Offset { get; }
        float BoardSize { get; }
    }
}