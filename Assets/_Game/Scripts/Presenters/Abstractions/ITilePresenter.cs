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
        bool IsTileType(TileColor tileType);
        TileColor GetTileType();
        void Initialize(TileColor tileType, string spriteName, int xIndex, int yIndex);
        Vector3 GetPosition();
        void SetPosition(int xIndex, int yIndex);
        UniTask SwipeToAsync(Vector3 targetPosition, CancellationToken cancellationToken = default);
        SwipeDirection SwipeDirection { get; set; }
        int GetXIndex();
        int GetYIndex();
        UniTask ScaleDownAsync(CancellationToken cancellationToken = default);
        UniTask MoveDownAsync(int newYPosition, float collapseTime, CancellationToken cancellationToken = default);
        void Release();
    }
}