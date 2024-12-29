using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class JellyManager : MonoBehaviour
    {
        [SerializeField] private DropManager dropManager;
        [SerializeField] private JellyController jellyController;
        [SerializeField] private Transform jellyParent;
        [SerializeField] private Transform newJellyPositioner;
        [SerializeField] private List<Jelly> randomJellies;
        
        public JellyController CreateJellyController(Jelly jelly, Vector3 jellyPosition)
        {
            var newJellyController = Instantiate(jellyController, jellyPosition, Quaternion.identity, jellyParent);
            newJellyController.CreateJellyParts(jelly);
            return newJellyController;
        }

        public void CreateNewJelly()
        {
            var randomJelly = CreateRandomJelly();
            var newJellyController = CreateJellyController(randomJelly, newJellyPositioner.position);
            dropManager.SetCurrentJellyController(newJellyController);
        }

        private Jelly CreateRandomJelly()
        {
            var availableJellyPartTypes = Enum.GetValues(typeof(JellyPartType)).Cast<JellyPartType>().ToList();
            availableJellyPartTypes.Remove(JellyPartType.None);
            
            var newJelly = new Jelly();
            var randomJelly = randomJellies[Random.Range(0, randomJellies.Count)];
            newJelly.jellyParts = new List<JellyPart>(randomJelly.jellyParts);
            foreach (var jellyPart in newJelly.jellyParts)
            {
                jellyPart.type = availableJellyPartTypes[Random.Range(0, availableJellyPartTypes.Count)];
                availableJellyPartTypes.Remove(jellyPart.type);
            }
            
            return newJelly;
        }
    }
}
