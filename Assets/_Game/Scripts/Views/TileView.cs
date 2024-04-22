using System;
using _Game.Scripts.Models;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

namespace _Game.Scripts.Views
{
    public enum SwipeDirection
    {
        Right,
        Up,
        Left,
        Down,
        Corner,
        None
    }

    public class TileView : MonoBehaviour, ITileView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public float AngleOffset => angleOffset;
        public float SwipeThreshold => swipeThreshold;
        public Action<PointerEventData> OnBeginDragAction { get; set; }
        public Action<PointerEventData> OnDragAction { get; set; }
        public Action<PointerEventData> OnEndDragAction { get; set; }

        [Header("References")] [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField] private SpriteAtlas spriteAtlas;

        [Header("Parameters"), Space(10)] [SerializeField]
        private float angleOffset = 30f;

        [SerializeField] private float swipeThreshold = 20f;

        public Transform Transform => gameObject.transform;

        public void Initialize(string spriteName)
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite(spriteName);
        }

        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            UnregisterEvents();
            spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }

        public void OnReset()
        {
            UnregisterEvents();
            spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragAction?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragAction?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragAction?.Invoke(eventData);
        }

        private void UnregisterEvents()
        {
            OnBeginDragAction = null;
            OnDragAction = null;
            OnEndDragAction = null;
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }
    }
}