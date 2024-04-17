using _Game.Scripts.Components.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Components
{
    public class TileSlotComponent : MonoBehaviour, ITileSlotComponent
    {
        public Transform Transform => gameObject.transform;
        public int XIndex { get; private set; }
        public int YIndex { get; private set; }

        public void Initialize(int xIndex, int yIndex)
        {
            XIndex = xIndex;
            YIndex = yIndex;
        }

        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void OnReset()
        {
            gameObject.SetActive(false);
        }
    }
}