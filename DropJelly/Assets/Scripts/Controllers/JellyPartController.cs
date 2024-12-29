using Data;
using UnityEngine;

namespace Controllers
{
    [System.Serializable]
    public class JellyPartController : MonoBehaviour
    {
        public JellyPartType type;
        public JellySizeType size;

        public void Initialize(JellyPart jellyPart)
        {
            type = jellyPart.type;
            size = jellyPart.size;
        }
    }
}
