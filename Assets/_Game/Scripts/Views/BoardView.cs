using _Game.Scripts.Views.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Views
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        [SerializeField] private GameObject tileSlotPrefab;
        [SerializeField] private Transform tileSlotParent;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float offset;
        
        public GameObject TileSlotPrefab => tileSlotPrefab;
        public Transform TileSlotParent => tileSlotParent;
        public int Width => width;
        public int Height => height;
        public float Offset => offset;
    }
}