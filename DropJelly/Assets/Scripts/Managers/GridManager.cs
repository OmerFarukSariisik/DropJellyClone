using Controllers;
using Data;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private CellController gridObject;
        [SerializeField] private Transform gridParent;
        
        private CellController[,] _gridData;

        public void InitializeGrid(int rowCount, int columnCount)
        {
            _gridData = new CellController[rowCount, columnCount];
        }

        public void InstantiateCell(Vector3 position, int row, int column)
        {
            _gridData[row, column] = Instantiate(gridObject, position, Quaternion.identity, gridParent);
        }

        public void SetGridItem(Cell cell, int row, int column)
        {
            _gridData[row, column].item = cell.item;
        }
    }
}
