using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "DropJelly/Level", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int targetCount;
        public int moveCount;
        public int rows;
        public int columns;
        public List<Cell> grid;

        public void InitializeGrid()
        {
            var cellCount = rows * columns;
            if (grid != null && grid.Count == cellCount) return;

            grid = new List<Cell>(cellCount);
            for (var i = 0; i < cellCount; i++)
                grid.Add(new Cell());
        }
    }

    [System.Serializable]
    public class Cell
    {
        public Jelly jelly;
        public CellItem item;
    }

    public enum CellItem
    {
        None = 0,
        Jelly = 1
    }

    [System.Serializable]
    public class JellyPart
    {
        public JellyPartType type;
        public JellySizeType size;
    }

    [System.Serializable]
    public class Jelly
    {
        public List<JellyPart> jellyParts = new();
    }


    public enum JellyPartType
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4
    }

    public enum JellySizeType
    {
        None = 0,
        LeftTop = 1,
        RightTop = 2,
        LeftBottom = 3,
        RightBottom = 4,
        Top = 5,
        Left = 6,
        Right = 7,
        Bottom = 8,
        Whole = 9,
    }

    public enum Direction
    {
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3
    }
}