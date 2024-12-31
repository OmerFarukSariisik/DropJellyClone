using System;
using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using UI;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private JellyManager jellyManager;
        [SerializeField] private DropManager dropManager;
        [SerializeField] private TopPanel topPanel;
        [SerializeField] private LevelEndPopup levelEndPopup;
        
        private CancellationTokenSource lifetimeCts = new();
        private int targetCount = 0;
        private int moveCount = 0;
        private bool isWaitingForMerge;
        
        private void Start()
        {
            LevelDataLoader.Instance.OnLevelLoaded += OnLevelLoaded;
            dropManager.OnTouchEnded += DropJelly;
            jellyManager.OnMerge += OnMerge;
            var currentLevel = LevelProgressSaver.Instance.GetCurrentLevel();
            LevelDataLoader.Instance.LoadLevelData(currentLevel);
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            targetCount = levelData.targetCount;
            moveCount = levelData.moveCount;
            topPanel.UpdateMoveCount(moveCount);
            topPanel.UpdateTargetCount(targetCount);
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
                            var jelly = jellyManager.CreateJellyController(cell.jelly, position, i, j);
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
            while (moveCount > 0)
            {
                isWaitingForMerge = true;
                jellyManager.CreateNewJelly();
                dropManager.SetInputLock(false);
                await UniTask.WaitUntil(() => !isWaitingForMerge, cancellationToken: lifetimeCts.Token);
            }
            
            levelEndPopup.LoseLevel();
        }
        
        private void DropJelly(JellyController jellyController, int columnIndex)
        {
            DropJellyAsync(jellyController, columnIndex).Forget();
            moveCount--;
            topPanel.UpdateMoveCount(moveCount);
        }
        private async UniTask DropJellyAsync(JellyController jellyController, int columnIndex)
        {
            if (!await gridManager.DropJelly(jellyController, columnIndex))
                levelEndPopup.LoseLevel();
            isWaitingForMerge = false;
        }
        
        private void OnMerge(int mergeCount)
        {
            if (lifetimeCts.IsCancellationRequested)
                return;
            targetCount -= mergeCount;
            topPanel.UpdateTargetCount(targetCount);

            if (targetCount <= 0)
            {
                LevelProgressSaver.Instance.SetLevelCompleted();
                levelEndPopup.WinLevel();
            }
        }

        private void OnDestroy()
        {
            lifetimeCts?.Cancel();
            jellyManager.OnMerge -= OnMerge;
            dropManager.OnTouchEnded -= DropJelly;
            LevelDataLoader.Instance.OnLevelLoaded -= OnLevelLoaded;
        }
    }
}
    