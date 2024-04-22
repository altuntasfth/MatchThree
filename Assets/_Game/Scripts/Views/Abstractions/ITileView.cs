using System;
using _Game.Scripts.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Views.Abstractions
{
    public interface ITileView
    {
        Transform Transform { get; }
        void Initialize(string spriteName);
        void OnGet();
        void OnRelease();
        void OnReset();
        float AngleOffset { get; }
        float SwipeThreshold { get; }
        Action<PointerEventData> OnBeginDragAction { get; set; }
        Action<PointerEventData> OnDragAction { get; set; }
        Action<PointerEventData> OnEndDragAction { get; set; }
    }
}