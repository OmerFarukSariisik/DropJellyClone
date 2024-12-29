using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data
{
    public class LevelDataLoader : MonoBehaviour
    {
        public static LevelDataLoader Instance { get; private set; }
        public event Action<LevelData> OnLevelLoaded;

        private string path = "Assets/Addressables/Level";

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void LoadLevelData(int levelNumber)
        {
            var assetPath = $"{path}{levelNumber}.asset";
            Addressables.LoadAssetAsync<LevelData>(assetPath).Completed += LevelLoaded;
        }

        private void LevelLoaded(AsyncOperationHandle<LevelData> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log(handle.Result);
                OnLevelLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError("Failed to load level data, going to level 1");
                LevelProgressSaver.Instance.ResetLevelData();
                LoadLevelData(1);
            }
        }
    }
}