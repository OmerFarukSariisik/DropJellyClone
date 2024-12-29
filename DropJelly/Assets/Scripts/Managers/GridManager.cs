using System.Collections.Generic;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private CellController gridObject;
        [SerializeField] private Transform gridParent;
        
        private CellController[,] gridData;

        public void InitializeGrid(int rowCount, int columnCount)
        {
            gridData = new CellController[rowCount, columnCount];
        }

        public void InstantiateCell(Vector3 position, int row, int column)
        {
            gridData[row, column] = Instantiate(gridObject, position, Quaternion.identity, gridParent);
        }

        public void SetGridItem(Cell cell, int row, int column)
        {
            gridData[row, column].SetCellItem(cell.item);
        }
        public void SetJellyController(JellyController jellyController, int row, int column)
        {
            gridData[row, column].SetJellyController(jellyController);
        }

        public List<Vector3> GetHorizontalPositions()
        {
            var positions = new List<Vector3>();
            for (var i = 0; i < gridData.GetLength(1); i++)
                positions.Add(gridData[0, i].transform.position);
            
            return positions;
        }

        public async UniTask DropJelly(JellyController jellyController, int columnIndex)
        {
            var (row, column) = GetAvailableCell(columnIndex);
            gridData[row, column].SetJellyControllerAndCellItem(jellyController, CellItem.Jelly);
            await jellyController.MoveToPositionAsync(gridData[row, column].transform.position);
            await CheckJellyAsync(new List<JellyController> { jellyController });
        }

        private async UniTask CheckJellyAsync(List<JellyController> jellyControllers)
        {
            //Check around all these jellies
            //If no match return
            
            //Merge and destroy same colors
            //Scale remaining parts or drop if destroyed
            //CheckJellyAsync();
        }

        private (int row, int column) GetAvailableCell(int columnIndex)
        {
            var (row, column) = (-1, -1);
            for (var i = gridData.GetLength(0) - 1; i >= 0 ; i--)
            {
                if (gridData[i, columnIndex].CellItem == CellItem.Jelly)
                    break;

                (row, column) = (i, columnIndex);
            }
            
            return (row, column);
        }
    }
}
