using System;
using System.Threading;
using System.Threading.Tasks;
using _Game.Scripts.Models;
using _Game.Scripts.Views;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Scripts.Presenters.Abstractions
{
    public interface ITilePresenter
    {
        Action<ITilePresenter> OnSwipeAction { get; set; }
        bool IsTileType(TileType tileType);
        TileType GetTileType();
        void Initialize(TileType tileType, string spriteName, int xIndex, int yIndex);
        Vector3 GetPosition();
        UniTask SwipeToAsync(Vector3 targetPosition, float duration, CancellationToken cancellationToken = default);
        SwipeDirection SwipeDirection { get; set; }
        int GetXIndex();
        int GetYIndex();
    }
}