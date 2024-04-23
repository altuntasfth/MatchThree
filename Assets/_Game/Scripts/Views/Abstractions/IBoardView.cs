using UnityEngine;

namespace _Game.Scripts.Views.Abstractions
{
    public interface IBoardView
    {
        GameObject TileSlotPrefab { get; }
        Transform TileSlotParent { get; }
        GameObject TilePrefab { get; }
        Transform TileParent { get; }
        float Offset { get; }
        float BoardSize { get; }
    }
}