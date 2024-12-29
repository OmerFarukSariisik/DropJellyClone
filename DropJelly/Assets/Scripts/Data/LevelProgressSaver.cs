using UnityEngine;

namespace Data
{
    public class LevelProgressSaver : MonoBehaviour
    {
        public static LevelProgressSaver Instance { get; private set; }
        private const string LevelKey = "CurrentLevel";

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public int GetCurrentLevel()
        {
            return PlayerPrefs.GetInt(LevelKey, 1);
        }

        public void SetLevelCompleted()
        {
            var lastLevel = PlayerPrefs.GetInt(LevelKey, 1);
            PlayerPrefs.SetInt(LevelKey, lastLevel + 1);
        }

        public void ResetLevelData()
        {
            PlayerPrefs.DeleteKey(LevelKey);
        }
    }
}