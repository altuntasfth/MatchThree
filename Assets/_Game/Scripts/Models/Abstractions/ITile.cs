namespace _Game.Scripts.Models.Abstractions
{
    public interface ITile
    {
        TileType TileType { get; set; }
        int XIndex { get; set; }
        int YIndex { get; set; }
        void SetPosition(int xIndex, int yIndex);
    }
}