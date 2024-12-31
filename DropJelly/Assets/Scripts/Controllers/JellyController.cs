using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class JellyController : MonoBehaviour
    {
        [SerializeField] private JellyPartController jellyPartControllerPrefab;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform wholeParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform topParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform bottomParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform leftParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform rightParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform rightTopParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform rightBottomParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform leftTopParent;

        [SerializeField, FoldoutGroup("Part Parents")]
        private Transform leftBottomParent;

        public List<JellyPartController> jellyParts;

        [ShowInInspector, ReadOnly] private int rowIndex;
        [ShowInInspector, ReadOnly] private int columnIndex;
        
        private CancellationTokenSource lifetimeCts = new();

        public readonly HashSet<JellySizeType> SmallSizeTypes = new()
            { JellySizeType.LeftTop, JellySizeType.RightTop, JellySizeType.LeftBottom, JellySizeType.RightBottom };

        public readonly HashSet<JellySizeType> BigSizeTypes = new()
            { JellySizeType.Top, JellySizeType.Right, JellySizeType.Left, JellySizeType.Bottom };

        public void CreateJellyParts(Jelly jelly)
        {
            jellyParts = new List<JellyPartController>();
            foreach (var jellyPart in jelly.jellyParts)
            {
                var jellyPartParent = GetParentForSize(jellyPart.size);
                var jellyPartController = Instantiate(jellyPartControllerPrefab, jellyPartParent);
                jellyPartController.Initialize(jellyPart);
                jellyParts.Add(jellyPartController);
            }
        }

        private Transform GetParentForSize(JellySizeType jellySizeType)
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

        public void SetRowAndColumn(int row, int column)
        {
            rowIndex = row;
            columnIndex = column;
        }

        public (int row, int column) GetRowAndColumn()
        {
            return (rowIndex, columnIndex);
        }

        public async UniTask MoveToPositionAsync(Vector3 position)
        {
            await transform.DOMove(position, 1f).WithCancellation(lifetimeCts.Token);
        }

        public void RemoveJellyParts(IEnumerable<JellyPartController> jellyPartsToRemove)
        {
            jellyParts.RemoveAll(jellyPartsToRemove.Contains);
        }

        public UniTask FillInTheBlanks()
        {
            JellyPartController part;
            switch (jellyParts.Count)
            {
                case 1:
                    //Make it whole part
                    part = jellyParts[0];
                    part.size = JellySizeType.Whole;
                    return part.FillTheBlank(wholeParent);
                case 2:
                    //If both are small parts, scale both
                    //else scale small part
                    if (SmallSizeTypes.Contains(jellyParts[0].size) && SmallSizeTypes.Contains(jellyParts[1].size))
                    {
                        var result =
                            JellyMatchChecker.instance.TransformSmallJellyParts(jellyParts[0].size, jellyParts[1].size);

                        jellyParts[0].size = result.size1;
                        jellyParts[0].FillTheBlank(GetParentForSize(result.size1));
                        jellyParts[1].size = result.size2;
                        return jellyParts[1].FillTheBlank(GetParentForSize(result.size2));
                    }

                    var smallPart = jellyParts.First(x => SmallSizeTypes.Contains(x.size));
                    var largePart = jellyParts.First(x => !SmallSizeTypes.Contains(x.size));

                    var sizeMapping = new Dictionary<JellySizeType, JellySizeType>
                    {
                        { JellySizeType.Top, JellySizeType.Bottom },
                        { JellySizeType.Left, JellySizeType.Right },
                        { JellySizeType.Right, JellySizeType.Left },
                        { JellySizeType.Bottom, JellySizeType.Top }
                    };

                    smallPart.size = sizeMapping[largePart.size];
                    return smallPart.FillTheBlank(GetParentForSize(smallPart.size));
                case 3:
                    var blankSizeType = FindBlankSizeType(jellyParts.Select(x => x.size).ToList());
                    switch (blankSizeType)
                    {
                        case JellySizeType.LeftTop:
                            part = jellyParts.First(x => x.size == JellySizeType.LeftBottom);
                            part.size = JellySizeType.Left;
                            return part.FillTheBlank(leftParent);
                        case JellySizeType.RightTop:
                            part = jellyParts.First(x => x.size == JellySizeType.RightBottom);
                            part.size = JellySizeType.Right;
                            return part.FillTheBlank(rightParent);
                        case JellySizeType.LeftBottom:
                            part = jellyParts.First(x => x.size == JellySizeType.RightBottom);
                            part.size = JellySizeType.Bottom;
                            return part.FillTheBlank(bottomParent);
                        case JellySizeType.RightBottom:
                            part = jellyParts.First(x => x.size == JellySizeType.LeftBottom);
                            part.size = JellySizeType.Bottom;
                            return part.FillTheBlank(bottomParent);
                    }
                    break;
            }
            return new UniTask();
        }

        public JellySizeType FindBlankSizeType(List<JellySizeType> jellySizeTypes)
        {
            foreach (var smallSizeType in SmallSizeTypes)
                if (!jellySizeTypes.Contains(smallSizeType))
                    return smallSizeType;

            throw new NotImplementedException("No small size type found");
        }

        private void OnDestroy()
        {
            lifetimeCts?.Cancel();
        }
    }
}