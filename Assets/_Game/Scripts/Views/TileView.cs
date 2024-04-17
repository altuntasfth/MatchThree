using _Game.Scripts.Models;
using _Game.Scripts.Views.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Views
{
    public class TileView : MonoBehaviour, ITileView
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite yellowSprite;
        
        public Transform Transform => gameObject.transform;
        
        public void Initialize(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.RED:
                    spriteRenderer.sprite = redSprite;
                    break;
                case TileType.GREEN:
                    spriteRenderer.sprite = greenSprite;
                    break;
                case TileType.BLUE:
                    spriteRenderer.sprite = blueSprite;
                    break;
                case TileType.YELLOW:
                    spriteRenderer.sprite = yellowSprite;
                    break;
            }
        }
        
        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }

        public void OnReset()
        {
            spriteRenderer.sprite = null;
            gameObject.SetActive(false);
        }
    }
}