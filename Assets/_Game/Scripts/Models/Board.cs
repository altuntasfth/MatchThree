using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Models
{
    public struct TileProp
    {
        public ITile Tile { get; set; }
        public ITilePresenter TilePresenter { get; set; }
        
        public TileProp(ITile tile, ITilePresenter tilePresenter)
        {
            Tile = tile;
            TilePresenter = tilePresenter;
        }
        
        public bool IsNull()
        {
            return Tile == null || TilePresenter == null;
        }
    }
    
    public class Board : IBoard
    {
        public ITileSlotComponent[,] TileSlots { get; set; }
        public TileProp[,] TileProps { get; set; }
        public int Width { get; }
        public int Height { get; }
        
        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            TileSlots = new ITileSlotComponent[Width, Height];
            TileProps = new TileProp[Width, Height];
        }
        
        public List<ITilePresenter> FindMatchesAt(int x, int y, int minLength = 3)
        {
            var horizMatches = FindHorizontalMatches(x, y, minLength);
            var vertMatches = FindVerticalMatches(x, y, minLength);

            horizMatches ??= new List<ITilePresenter>();
            vertMatches ??= new List<ITilePresenter>();
            
            var combinedMatches = horizMatches.Union(vertMatches).ToList();
            return combinedMatches;
        }
        
        private List<ITilePresenter> FindMatchesAt(List<ITilePresenter> tilePresenters, int minLength = 3)
        {
            var matches = new List<ITilePresenter>();

            foreach (var presenter in tilePresenters)
            {
                matches = matches.Union(FindMatchesAt(presenter.GetXIndex(), presenter.GetYIndex(), minLength)).ToList();
            }
            
            return matches;
        }
        
        private List<ITilePresenter> FindVerticalMatches(int startX, int startY, int minLength = 3)
        {
            var upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
            var downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

            upwardMatches ??= new List<ITilePresenter>();
            downwardMatches ??= new List<ITilePresenter>();
            var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

            return (combinedMatches.Count >= minLength) ? combinedMatches : null;
        }

        private List<ITilePresenter> FindHorizontalMatches(int startX, int startY, int minLength = 3)
        {
            var rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
            var leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

            rightMatches ??= new List<ITilePresenter>();
            leftMatches ??= new List<ITilePresenter>();
            var combinedMatches = rightMatches.Union(leftMatches).ToList();

            return (combinedMatches.Count >= minLength) ? combinedMatches : null;
        }
        
        private List<ITilePresenter> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
        {
            var matches = new List<ITilePresenter>();
            ITilePresenter startPiece = null;

            if (IsWithinBounds(startX, startY))
                startPiece = TileProps[startX, startY].TilePresenter;
            
            if (startPiece != null)
                matches.Add(startPiece);
            else
                return null;

            var maxValue = (Width > Height) ? Width : Height;

            for (var i = 1; i < maxValue - 1; i++)
            {
                var nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
                var nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

                if (!IsWithinBounds(nextX, nextY))
                    break;
                
                var nextPiece = TileProps[nextX, nextY].TilePresenter;

                if (nextPiece == null)
                    break;
                else
                {
                    if (nextPiece.IsTileType(startPiece.GetTileType()) && 
                        !matches.Contains(nextPiece) && !nextPiece.IsTileType(TileType.NONE))
                        matches.Add(nextPiece);
                    else
                        break;
                }
            }

            return matches.Count >= minLength ? matches : null;
        }
        
        private bool IsWithinBounds(int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height);
        }
        
        
        
        private List<ITilePresenter> CollapseColumn(List<int> columnsToCollapse)
        {
            var movingPieces = new List<ITilePresenter>();
            foreach (int column in columnsToCollapse)
            {
                movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
            }
            return movingPieces;
        }
        
        private List<int> GetColumns(List<ITilePresenter> tilePresenters)
        {
            List<int> columns = new List<int>();

            foreach (ITilePresenter piece in tilePresenters)
            {
                if (piece != null)
                {
                    if (!columns.Contains(piece.GetXIndex()))
                    {
                        columns.Add(piece.GetXIndex());
                    }
                }
            }
            return columns;
        }
    }
}
