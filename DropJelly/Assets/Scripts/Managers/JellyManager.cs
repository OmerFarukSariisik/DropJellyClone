using Controllers;
using Data;
using UnityEngine;

namespace Managers
{
    public class JellyManager : MonoBehaviour
    {
        [SerializeField] private JellyController jellyController;
        [SerializeField] private Transform jellyParent;
        
        public JellyController CreateJellyController(Jelly jelly)
        {
            var newJellyController = Instantiate(jellyController, jellyParent);
            newJellyController.CreateJellyParts(jelly);
            return newJellyController;
        }
    }
}
