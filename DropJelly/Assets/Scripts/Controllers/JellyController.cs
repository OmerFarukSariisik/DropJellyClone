using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class JellyController : MonoBehaviour
    {
        [SerializeField] private JellyPartController jellyPartControllerPrefab;
        public List<JellyPartController> jellyParts;

        public void CreateJellyParts(Jelly jelly)
        {
            jellyParts = new List<JellyPartController>();
            foreach (var jellyPart in jelly.jellyParts)
            {
                var jellyPartController = Instantiate(jellyPartControllerPrefab, transform);
                jellyPartController.Initialize(jellyPart);
                jellyParts.Add(jellyPartController);
            }
        }
    }
}
