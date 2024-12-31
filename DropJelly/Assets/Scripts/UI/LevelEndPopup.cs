using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelEndPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private Button nextButton;
        
        private void Awake()
        {
            nextButton.onClick.AddListener(OnButtonClick);
        }

        public void WinLevel()
        {
            gameObject.SetActive(true);
            titleText.text = "You Win!";
            buttonText.text = "Next";
        }

        public void LoseLevel()
        {
            gameObject.SetActive(true);
            titleText.text = "You Lose!";
            buttonText.text = "Retry";
        }

        private void OnButtonClick()
        {
            SceneManager.LoadScene(0);
        }

        private void OnDestroy()
        {
            nextButton.onClick.RemoveAllListeners();
        }
    }
}
