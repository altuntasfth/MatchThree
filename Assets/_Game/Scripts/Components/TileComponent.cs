using _Game.Scripts.Components.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Components
{
    public enum TileType
    {
        RED,
        GREEN,
        BLUE,
        YELLOW
    }
    
    public class TileComponent : MonoBehaviour, ITileComponent
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite yellowSprite;

        public TileType TileType { get; set; }
        public Transform Transform => gameObject.transform;

        public void Initialize(TileType tileType)
        {
            TileType = tileType;
            switch (TileType)
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