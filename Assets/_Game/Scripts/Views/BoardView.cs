using _Game.Scripts.Views.Abstractions;
using UnityEngine;

namespace _Game.Scripts.Views
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        [SerializeField] private GameObject tileSlotPrefab;
        [SerializeField] private Transform tileSlotParent;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform tileParent;
        [SerializeField] private float offset;
        [SerializeField] private int boardSize;
        
        public GameObject TileSlotPrefab => tileSlotPrefab;
        public Transform TileSlotParent => tileSlotParent;
        public GameObject TilePrefab => tilePrefab;
        public Transform TileParent => tileParent;
        public float Offset => offset;
        public float BoardSize => boardSize;
    }
}