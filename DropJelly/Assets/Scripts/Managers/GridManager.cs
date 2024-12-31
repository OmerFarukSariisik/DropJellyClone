using System.Collections.Generic;
using System.Linq;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private JellyManager jellyManager;
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

        public async UniTask<bool> DropJelly(JellyController jellyController, int columnIndex)
        {
            var (row, column) = GetAvailableCell(columnIndex);
            if (row == -1)
                return false;
            gridData[row, column].SetJellyControllerAndCellItem(jellyController, CellItem.Jelly);
            jellyController.SetRowAndColumn(row, column);
            await jellyController.MoveToPositionAsync(gridData[row, column].transform.position);
            await CheckJellyAsync(new List<JellyController> { jellyController });
            return true;
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
        
        private async UniTask CheckJellyAsync(List<JellyController> jellyControllers)
        {
            var effectedJellyControllers = new HashSet<JellyController>();
            var fallenJellyControllers = new HashSet<JellyController>();

            //Check around all these jellies
            foreach (var jellyController in jellyControllers)
                CheckForMerge(jellyController, effectedJellyControllers);
            
            //If no match return
            if (effectedJellyControllers.Count == 0)
                return;

            //Merge and destroy same colors
            await jellyManager.MergeJellyPartsAsync(effectedJellyControllers);
            
            //Fall to empty cells
            var destroyedIndexes = jellyManager.DestroyJellyIndexes(effectedJellyControllers);
            var fallTasks = CheckForFall(destroyedIndexes, fallenJellyControllers);
            
            //Scale remaining parts
            await jellyManager.FillInTheBlanks(effectedJellyControllers);
            await UniTask.WhenAll(fallTasks);
            var effectedList = effectedJellyControllers.ToList();
            effectedList.AddRange(fallenJellyControllers);
            await CheckJellyAsync(effectedList);
        }

        private void CheckForMerge(JellyController jellyController, HashSet<JellyController> effectedJellyControllers)
        {
            var (row, column) = jellyController.GetRowAndColumn();

            if (CheckValidity(row + 1, column))
                jellyManager.CheckForMerge(jellyController, gridData[row + 1, column].JellyController, Direction.Up, effectedJellyControllers);
            if (CheckValidity(row - 1, column))
                jellyManager.CheckForMerge(jellyController, gridData[row - 1, column].JellyController, Direction.Down, effectedJellyControllers);
            if (CheckValidity(row, column + 1))
                jellyManager.CheckForMerge(jellyController, gridData[row, column + 1].JellyController, Direction.Right, effectedJellyControllers);
            if (CheckValidity(row, column - 1))
                jellyManager.CheckForMerge(jellyController, gridData[row, column - 1].JellyController, Direction.Left, effectedJellyControllers);
            
        }

        private bool CheckValidity(int row, int column)
        {
            var isValid = row >= 0 && row < gridData.GetLength(0) && column >= 0 && column < gridData.GetLength(1);
            return isValid && gridData[row, column].CellItem == CellItem.Jelly;
        }

        private List<UniTask> CheckForFall(List<(int row, int column)> indexes,  HashSet<JellyController> effectedJellyControllers)
        {
            var fallTasks = new List<UniTask>();
            foreach (var index in indexes)
            {
                for (var i = index.row + 1; i < gridData.GetLength(0); i++)
                {
                    if (gridData[i, index.column].CellItem == CellItem.None)
                    {
                        if (i == index.row + 1)
                            gridData[index.row, index.column].SetJellyControllerAndCellItem(null, CellItem.None);
                        break;
                    }

                    var jellyController = gridData[i, index.column].JellyController;
                    gridData[i, index.column].SetJellyControllerAndCellItem(null, CellItem.None);
                    gridData[i - 1, index.column].SetJellyControllerAndCellItem(jellyController, CellItem.Jelly);
                    var targetPos = gridData[i - 1, index.column].transform.position;
                    jellyController.SetRowAndColumn(i - 1, index.column);
                    fallTasks.Add(jellyController.MoveToPositionAsync(targetPos));
                    effectedJellyControllers.Add(jellyController);
                }
            }
            
            return fallTasks;
        }
    }
}
