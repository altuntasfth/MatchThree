using System;
using System.Collections.Generic;
using _Game.Scripts.Models;
using _Game.Scripts.Presenters;
using _Game.Scripts.Presenters.Abstractions;
using _Game.Scripts.Views;
using UnityEngine;

namespace _Game.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private BoardView boardView;
        
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private List<int> nonSpawnerRows = new List<int> {2, 6};
        
        private IBoardPresenter _boardPresenter;

        private void Start()
        {
            _boardPresenter = new BoardPresenter(boardView, new Board(width, height, nonSpawnerRows));
            _boardPresenter.Initialize();
            playerManager.Initialize(_boardPresenter);
        }
    }
}