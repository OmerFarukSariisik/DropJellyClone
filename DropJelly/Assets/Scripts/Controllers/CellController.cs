using Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    public class CellController : MonoBehaviour
    {
        public JellyController JellyController { get; private set; }
        public CellItem CellItem { get; private set; }

        public void SetJellyController(JellyController controller)
        {
            JellyController = controller;
        }

        public void SetCellItem(CellItem item)
        {
            CellItem = item;
        }

        public void SetJellyControllerAndCellItem(JellyController controller, CellItem item)
        {
            SetJellyController(controller);
            SetCellItem(item);
        }
    }
}
