using _Game.Scripts.Models.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Models
{
    public enum TileColor
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        NONE
    }
    
    public class Tile : ITile
    {
        public bool IsMoving { get; set; }
        public TileColor TileColor { get; set; }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        
        public void SetPosition(int xIndex, int yIndex)
        {
            XIndex = xIndex;
            YIndex = yIndex;
        }

        public Vector3 GetPosition()
        {
            return Vector3.right * XIndex + Vector3.up * YIndex;
        }
    }
}