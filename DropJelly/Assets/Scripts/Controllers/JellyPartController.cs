using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
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

        public bool merge;

        private CancellationTokenSource lifetimeCts = new();

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

        public UniTask DoMerge()
        {
            return transform.DOScale(Vector3.zero, 1f).OnComplete(() => Destroy(gameObject))
                .ToUniTask(cancellationToken: lifetimeCts.Token);
        }

        public UniTask FillTheBlank(Transform newParent)
        {
            transform.SetParent(newParent);
            transform.DOLocalMove(Vector3.zero, 1f);
            return transform.DOScale(0.9f, 1f).ToUniTask(cancellationToken: lifetimeCts.Token);
        }

        private void OnDestroy()
        {
            lifetimeCts?.Cancel();
        }
    }
}