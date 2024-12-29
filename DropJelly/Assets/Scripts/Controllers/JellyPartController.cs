using System;
using Data;
using UnityEngine;

namespace Controllers
{
    [Serializable]
    public class JellyPartController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material redMaterial;
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material blueMaterial;
        [SerializeField] private Material yellowMaterial;
        
        public JellyPartType type;
        public JellySizeType size;

        public void Initialize(JellyPart jellyPart)
        {
            type = jellyPart.type;
            size = jellyPart.size;

            SetColor();
        }

        private void SetColor()
        {
            meshRenderer.material = type switch
            {
                JellyPartType.Red => redMaterial,
                JellyPartType.Green => greenMaterial,
                JellyPartType.Blue => blueMaterial,
                JellyPartType.Yellow => yellowMaterial,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
