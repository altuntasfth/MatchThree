using _Game.Scripts.Models.Abstractions;

namespace _Game.Scripts.Models
{
    public enum TileType
    {
        RED,
        GREEN,
        BLUE,
        YELLOW
    }
    
    public class Tile : ITile
    {
        public TileType TileType { get; set; }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        
        public void SetPosition(int xIndex, int yIndex)
        {
            XIndex = xIndex;
            YIndex = yIndex;
        }
    }
}