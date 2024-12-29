using UnityEditor;
using UnityEngine;

namespace Data.Editor
{
    [CustomEditor(typeof(LevelData))]
    public class LevelEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var levelData = (LevelData)target;

            // Input fields for rows, columns and goal.
            levelData.goal = EditorGUILayout.IntField("Goal", levelData.goal);
            levelData.rows = EditorGUILayout.IntField("Rows", levelData.rows);
            levelData.columns = EditorGUILayout.IntField("Columns", levelData.columns);

            if (GUILayout.Button("Initialize Grid"))
            {
                levelData.InitializeGrid();
            }
        
            if (levelData.grid != null && levelData.grid.Count == levelData.rows * levelData.columns)
            {
                // Draw the grid with dropdowns for each cell.
                for (var row = levelData.rows - 1; row >= 0; row--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var col = 0; col < levelData.columns; col++)
                    {
                        EditorGUILayout.BeginVertical("box");
                        var index = row * levelData.columns + col;

                        // Dropdown for selecting item types.
                        levelData.grid[index].item =
                            (CellItem)EditorGUILayout.EnumPopup(levelData.grid[index].item,
                                GUILayout.Width(120), GUILayout.Height(120));

                        if (levelData.grid[index].item == CellItem.Jelly)
                        {
                            for (int j = 0; j < levelData.grid[index].jelly.jellyParts.Count; j++)
                            {
                                JellyPart jellyPart = levelData.grid[index].jelly.jellyParts[j];
                            
                                jellyPart.type = (JellyPartType)EditorGUILayout.EnumPopup(jellyPart.type, GUILayout.Width(120));
                                jellyPart.size = (JellySizeType)EditorGUILayout.EnumPopup(jellyPart.size, GUILayout.Width(120));
                                if (GUILayout.Button("Remove Jelly Part", GUILayout.Width(120)))
                                {
                                    levelData.grid[index].jelly.jellyParts.RemoveAt(j);
                                    break;
                                }
                                GUILayout.Space(10);
                            }
                        
                            if (GUILayout.Button("Add Jelly Part", GUILayout.Width(120)))
                                levelData.grid[index].jelly.jellyParts.Add(new JellyPart());
                        }
                    
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Click 'Initialize Grid' to set up the grid.", MessageType.Warning);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(levelData); // Save changes in the editor.
            }
        }
    }
}
