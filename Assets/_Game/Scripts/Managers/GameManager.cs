using System;
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
        
        private IBoardPresenter _boardPresenter;

        private void Start()
        {
            _boardPresenter = new BoardPresenter(boardView, new Board());
            _boardPresenter.Initialize();
            playerManager.Initialize(_boardPresenter);
        }
    }
}