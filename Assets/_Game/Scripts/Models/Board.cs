using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Game.Scripts.Components.Abstractions;
using _Game.Scripts.Models.Abstractions;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views.Abstractions;
using Cysharp.Threading.Tasks;
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
        
        public List<ITilePresenter> FindAllMatches()
        {
            var combinedMatches = new List<ITilePresenter>();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var matches = FindMatchesAt(i, j);
                    combinedMatches = combinedMatches.Union(matches).ToList();
                }
            }
        
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
                        !matches.Contains(nextPiece))
                    {
                        matches.Add(nextPiece);
                    }
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
        
        public bool HasMatchOnFill(int x, int y, int minLength = 3)
        {
            var leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
            var downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

            leftMatches ??= new List<ITilePresenter>();
            downwardMatches ??= new List<ITilePresenter>();

            return (leftMatches.Count > 0 || downwardMatches.Count > 0);

        }

        public async UniTask ClearAndCollapseAsync(List<ITilePresenter> gamePieces)
        {
            var movingPieces = new List<ITilePresenter>();
            var matches = new List<ITilePresenter>();

            bool isFinished = false;

            while (!isFinished)
            {
                await ClearPieceAt(gamePieces);

                movingPieces = await CollapseColumn(gamePieces);

                await UniTask.Delay(200);

                matches = FindMatchesAt(movingPieces);

                if (matches.Count == 0)
                {
                    isFinished = true;
                }
                else
                {
                    gamePieces = matches;
                }
            }
        }
        
        public async UniTask ClearPieceAt(List<ITilePresenter> gamePieces)
        {
            string log = "Clearing pieces: ";
            var scaleDownTasks = new List<UniTask>();
            foreach (var piece in gamePieces.Where(piece => piece != null))
            {
                log += $"[{piece.GetXIndex()}, {piece.GetYIndex()}]: ${piece.GetTileType()} ";
                var scaleDownTask = ClearPieceAt(piece.GetXIndex(), piece.GetYIndex());
                scaleDownTasks.Add(scaleDownTask);
            }
            await UniTask.WhenAll(scaleDownTasks);
            Debug.Log(log);
        }
        
        public async UniTask ClearPieceAt(int x, int y)
        {
            var pieceToClear = TileProps[x, y].TilePresenter;
            if (pieceToClear == null) return;
            
            TileProps[x, y] = new TileProp(null, null);
            await pieceToClear.ScaleDownAsync();
        }
        
        private async UniTask<List<ITilePresenter>> CollapseColumn(List<ITilePresenter> tiles)
        {
            var movingPieces = new List<ITilePresenter>();
            var columnsToCollapse = GetColumns(tiles);

            foreach (int column in columnsToCollapse)
            {
                movingPieces = movingPieces.Union(await CollapseColumn(column)).ToList();
            }

            return movingPieces;
        }
        
        private async UniTask<List<ITilePresenter>> CollapseColumn(int column, float collapseTime = 0.1f)
        {
            var movingPieces = new List<ITilePresenter>();
            
            var moveDownTasks = new List<UniTask>();

            for (int i = 0; i < Height - 1; i++)
            {
                if (TileProps[column, i].TilePresenter == null)
                {
                    for (int j = i + 1; j < Height; j++)
                    {
                        if (TileProps[column, j].TilePresenter != null)
                        {
                            var moveDownTask = TileProps[column, j].TilePresenter.MoveDownAsync(i, collapseTime * (j - i));
                            moveDownTasks.Add(moveDownTask);
                            TileProps[column, i] = TileProps[column, j];
                            TileProps[column, i].TilePresenter.SetPosition(column, i);

                            if (!movingPieces.Contains(TileProps[column, i].TilePresenter))
                            {
                                movingPieces.Add(TileProps[column, i].TilePresenter);
                            }

                            TileProps[column, j] = new TileProp(null, null);
                            break;

                        }
                    }
                }
            }
            
            await UniTask.WhenAll(moveDownTasks);
            
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
