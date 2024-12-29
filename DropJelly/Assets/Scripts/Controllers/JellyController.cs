using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class JellyController : MonoBehaviour
    {
        [SerializeField] private JellyPartController jellyPartControllerPrefab;
        
        [SerializeField, FoldoutGroup("Part Parents")] private Transform wholeParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform topParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform bottomParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform leftParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform rightParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform rightTopParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform rightBottomParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform leftTopParent;
        [SerializeField, FoldoutGroup("Part Parents")] private Transform leftBottomParent;
        
        public List<JellyPartController> jellyParts;

        public void CreateJellyParts(Jelly jelly)
        {
            jellyParts = new List<JellyPartController>();
            foreach (var jellyPart in jelly.jellyParts)
            {
                var jellyPartParent = GetParent(jellyPart.size);
                var jellyPartController = Instantiate(jellyPartControllerPrefab, jellyPartParent);
                jellyPartController.Initialize(jellyPart);
                jellyParts.Add(jellyPartController);
            }
        }

        private Transform GetParent(JellySizeType jellySizeType)
        {
            return jellySizeType switch
            {
                JellySizeType.LeftTop => leftTopParent,
                JellySizeType.RightTop => rightTopParent,
                JellySizeType.LeftBottom => leftBottomParent,
                JellySizeType.RightBottom => rightBottomParent,
                JellySizeType.Top => topParent,
                JellySizeType.Left => leftParent,
                JellySizeType.Right => rightParent,
                JellySizeType.Bottom => bottomParent,
                JellySizeType.Whole => wholeParent,
                _ => throw new ArgumentOutOfRangeException(nameof(jellySizeType), jellySizeType, null)
            };
        }

        public void SetHorizontalPosition(Vector3 position)
        {
            position.y = transform.position.y;
            position.z = transform.position.z;
            transform.position = position;
        }

        public async UniTask MoveToPositionAsync(Vector3 position)
        {
            await transform.DOMove(position, 1f);
        }
    }
}
