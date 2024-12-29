using UnityEngine;
using System;
using System.Collections.Generic;
using Controllers;

namespace Managers
{
    public class DropManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        public event Action<JellyController, int> OnTouchEnded;
        
        private JellyController currentJellyController;
        private List<Vector3> columnPositions = new();
        private bool isLocked = true;

        private void Update()
        {
            if (isLocked || Input.touchCount == 0)
                return;
            
            var touch = Input.GetTouch(0);

            var position = mainCamera.ScreenToWorldPoint(touch.position);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                    
                    currentJellyController.SetHorizontalPosition(position);
                    break;
                case TouchPhase.Ended:
                    var columnIndex = GetClosestColumn(position);
                    currentJellyController.SetHorizontalPosition(columnPositions[columnIndex]);
                    OnTouchEnded?.Invoke(currentJellyController, columnIndex);
                    SetInputLock(true);
                    break;
            }
        }

        private int GetClosestColumn(Vector3 position)
        {
            var closestColumn = 0;
            var closestDistance = Mathf.Infinity;
            for (var i = 0; i < columnPositions.Count; i++)
            {
                var distance = Mathf.Abs(columnPositions[i].x - position.x);
                if (distance > closestDistance)
                    continue;
                closestDistance = distance;
                closestColumn = i;
            }
            
            return closestColumn;
        }

        public void SetHorizontalPositions(List<Vector3> positions)
        {
            columnPositions = positions;
        }

        public void SetInputLock(bool lockState)
        {
            isLocked = lockState;
        }

        public void SetCurrentJellyController(JellyController controller)
        {
            currentJellyController = controller;
        }
    }
}
