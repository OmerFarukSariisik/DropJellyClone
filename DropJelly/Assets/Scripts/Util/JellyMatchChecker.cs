using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Managers
{
    public class JellyMatchChecker : MonoBehaviour
    {
        public static JellyMatchChecker instance;

        private readonly HashSet<JellySizeType> bottomInTouchSizeTypes = new()
        {
            JellySizeType.LeftBottom, JellySizeType.RightBottom, JellySizeType.Left, JellySizeType.Right,
            JellySizeType.Bottom, JellySizeType.Whole
        };

        private readonly HashSet<JellySizeType> topInTouchSizeTypes = new()
        {
            JellySizeType.LeftTop, JellySizeType.RightTop, JellySizeType.Left, JellySizeType.Right, JellySizeType.Top,
            JellySizeType.Whole
        };

        private readonly HashSet<JellySizeType> rightInTouchSizeTypes = new()
        {
            JellySizeType.RightTop, JellySizeType.RightBottom, JellySizeType.Top, JellySizeType.Bottom,
            JellySizeType.Right, JellySizeType.Whole
        };

        private readonly HashSet<JellySizeType> leftInTouchSizeTypes = new()
        {
            JellySizeType.LeftTop, JellySizeType.LeftBottom, JellySizeType.Top, JellySizeType.Bottom,
            JellySizeType.Left, JellySizeType.Whole
        };


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        public bool AreJellyPartsInTouch(JellySizeType jellySizeType, JellySizeType otherJellySizeType,
            Direction direction)
        {
            bool opposite1;
            bool opposite2;
            bool isPartInTouch;

            switch (direction)
            {
                case Direction.Up:
                    opposite1 = jellySizeType is JellySizeType.LeftTop or JellySizeType.Left &&
                                otherJellySizeType is JellySizeType.RightBottom or JellySizeType.Right;
                    opposite2 = jellySizeType is JellySizeType.RightTop or JellySizeType.Right &&
                                otherJellySizeType is JellySizeType.LeftBottom or JellySizeType.Left;
                    isPartInTouch = topInTouchSizeTypes.Contains(jellySizeType) &&
                                    bottomInTouchSizeTypes.Contains(otherJellySizeType);

                    return !opposite1 && !opposite2 && isPartInTouch;
                case Direction.Down:
                    opposite1 = jellySizeType is JellySizeType.RightBottom or JellySizeType.Right &&
                                otherJellySizeType is JellySizeType.LeftTop or JellySizeType.Left;
                    opposite2 = jellySizeType is JellySizeType.LeftBottom or JellySizeType.Left &&
                                otherJellySizeType is JellySizeType.RightTop or JellySizeType.Right;
                    isPartInTouch = bottomInTouchSizeTypes.Contains(jellySizeType) &&
                                    topInTouchSizeTypes.Contains(otherJellySizeType);

                    return !opposite1 && !opposite2 && isPartInTouch;
                case Direction.Right:
                    opposite1 = jellySizeType is JellySizeType.RightBottom or JellySizeType.Bottom &&
                                otherJellySizeType is JellySizeType.LeftTop or JellySizeType.Top;
                    opposite2 = jellySizeType is JellySizeType.RightTop or JellySizeType.Top &&
                                otherJellySizeType is JellySizeType.LeftBottom or JellySizeType.Bottom;
                    isPartInTouch = rightInTouchSizeTypes.Contains(jellySizeType) &&
                                    leftInTouchSizeTypes.Contains(otherJellySizeType);

                    return !opposite1 && !opposite2 && isPartInTouch;
                case Direction.Left:
                    opposite1 = jellySizeType is JellySizeType.LeftBottom or JellySizeType.Bottom &&
                                otherJellySizeType is JellySizeType.RightTop or JellySizeType.Top;
                    opposite2 = jellySizeType is JellySizeType.LeftTop or JellySizeType.Top &&
                                otherJellySizeType is JellySizeType.RightBottom or JellySizeType.Bottom;
                    isPartInTouch = leftInTouchSizeTypes.Contains(jellySizeType) &&
                                    rightInTouchSizeTypes.Contains(otherJellySizeType);

                    return !opposite1 && !opposite2 && isPartInTouch;
            }

            return false;
        }

        public (JellySizeType size1, JellySizeType size2) TransformSmallJellyParts(JellySizeType size1,
            JellySizeType size2)
        {
            var (part1, part2) = (JellySizeType.LeftTop, JellySizeType.RightTop);

            var transformationRules = new Dictionary<(JellySizeType, JellySizeType), (JellySizeType, JellySizeType)>
            {
                {
                    (JellySizeType.LeftTop, JellySizeType.LeftBottom), (JellySizeType.Top, JellySizeType.Bottom)
                },
                {
                    (JellySizeType.LeftTop, JellySizeType.RightTop), (JellySizeType.Left, JellySizeType.Right)
                },
                {
                    (JellySizeType.LeftTop, JellySizeType.RightBottom), (JellySizeType.Left, JellySizeType.Right)
                },
                {
                    (JellySizeType.RightTop, JellySizeType.LeftTop), (JellySizeType.Right, JellySizeType.Left)
                },
                {
                    (JellySizeType.RightTop, JellySizeType.RightBottom), (JellySizeType.Top, JellySizeType.Bottom)
                },
                {
                    (JellySizeType.RightTop, JellySizeType.LeftBottom), (JellySizeType.Right, JellySizeType.Left)
                },
                {
                    (JellySizeType.LeftBottom, JellySizeType.LeftTop), (JellySizeType.Bottom, JellySizeType.Top)
                },
                {
                    (JellySizeType.LeftBottom, JellySizeType.RightBottom), (JellySizeType.Left, JellySizeType.Right)
                },
                {
                    (JellySizeType.LeftBottom, JellySizeType.RightTop), (JellySizeType.Bottom, JellySizeType.Top)
                },
                {
                    (JellySizeType.RightBottom, JellySizeType.LeftBottom), (JellySizeType.Right, JellySizeType.Left)
                },
                {
                    (JellySizeType.RightBottom, JellySizeType.RightTop), (JellySizeType.Bottom, JellySizeType.Top)
                },
                {
                    (JellySizeType.RightBottom, JellySizeType.LeftTop), (JellySizeType.Bottom, JellySizeType.Top)
                }
            };

            return transformationRules[(size1, size2)];
        }
    }
}