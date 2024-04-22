using UnityEngine;

namespace _Game.Scripts.Models.Abstractions
{
    public interface ITile
    {
        bool IsMoving { get; set; }
        TileType TileType { get; set; }
        int XIndex { get; set; }
        int YIndex { get; set; }
        void SetPosition(int xIndex, int yIndex);
        Vector3 GetPosition();
    }
}