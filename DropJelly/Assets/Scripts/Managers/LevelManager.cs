using System;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private JellyManager jellyManager;
        [SerializeField] private DropManager dropManager;
        
        private int _goalCount = 0;
        
        private void Start()
        {
            LevelDataLoader.Instance.OnLevelLoaded += OnLevelLoaded;
            dropManager.OnTouchEnded += DropJelly;
            var currentLevel = LevelProgressSaver.Instance.GetCurrentLevel();
            LevelDataLoader.Instance.LoadLevelData(currentLevel);
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            _goalCount = levelData.goal;
            gridManager.InitializeGrid(levelData.rows, levelData.columns);
            
            for (var i = 0; i < levelData.rows; i++)
            {
                for (var j = 0; j < levelData.columns; j++)
                {
                    var xPos = j - (levelData.columns - 1) / 2f;
                    var yPos = i - (levelData.rows - 1) / 2f;

                    var position = new Vector3(xPos, yPos, 0);
                    
                    var index = i * levelData.columns + j;
                    var cell = levelData.grid[index];

                    gridManager.InstantiateCell(position, i, j);
                    gridManager.SetGridItem(cell, i, j);

                    switch (cell.item)
                    {
                        case CellItem.Jelly:
                            var jelly = jellyManager.CreateJellyController(cell.jelly, position);
                            gridManager.SetJellyController(jelly, i, j);
                            break;
                    }
                }
            }

            dropManager.SetHorizontalPositions(gridManager.GetHorizontalPositions());
            StartGameCycle().Forget();
        }

        private async UniTask StartGameCycle()
        {
            // while (true)
            // {
                jellyManager.CreateNewJelly();
                dropManager.SetInputLock(false);
            // }
            
            
            //Create new jelly
            
            //Wait for input
            
            //Drop jelly and wait for match and animations
            
            //Check goal
        }
        
        private void DropJelly(JellyController jellyController, int columnIndex)
        {
            gridManager.DropJelly(jellyController, columnIndex);
        }

        private void OnDestroy()
        {
            dropManager.OnTouchEnded -= DropJelly;
            LevelDataLoader.Instance.OnLevelLoaded -= OnLevelLoaded;
        }
    }
}
    