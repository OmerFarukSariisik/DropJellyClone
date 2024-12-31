using TMPro;
using UnityEngine;

namespace UI
{
    public class TopPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text moveCountText;
        [SerializeField] private TMP_Text targetCountText;

        public void UpdateMoveCount(int moveCount)
        {
            moveCountText.text = moveCount.ToString();
        }
        
        public void UpdateTargetCount(int targetCount)
        {
            targetCount = Mathf.Max(0, targetCount);
            targetCountText.text = targetCount.ToString();
        }
    }
}
