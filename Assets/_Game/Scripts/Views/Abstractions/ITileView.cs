using System;
using _Game.Scripts.Models;
using UnityEngine;

namespace _Game.Scripts.Views.Abstractions
{
    public interface ITileView
    {
        Transform Transform { get; }
        void Initialize(string spriteName);
        void OnGet();
        void OnRelease();
        void OnReset();
    }
}