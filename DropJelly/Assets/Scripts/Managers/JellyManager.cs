using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Controllers;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
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

        public event Action<int> OnMerge;

        private CancellationTokenSource lifetimeCts = new();
        
        public JellyController CreateJellyController(Jelly jelly, Vector3 jellyPosition, int rowIndex, int columnIndex)
        {
            var newJellyController = Instantiate(jellyController, jellyPosition, Quaternion.identity, jellyParent);
            newJellyController.CreateJellyParts(jelly);
            newJellyController.SetRowAndColumn(rowIndex, columnIndex);
            return newJellyController;
        }

        public void CreateNewJelly()
        {
            var randomJelly = CreateRandomJelly();
            var newJellyController = CreateJellyController(randomJelly, newJellyPositioner.position, -1, -1);
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

        public void CheckForMerge(JellyController jelly, JellyController otherJelly, Direction direction,
            HashSet<JellyController> effectedJellyControllers)
        {
            if (otherJelly is null)
                return;

            foreach (var jellyPart in jelly.jellyParts)
            {
                foreach (var otherJellyPart in otherJelly.jellyParts)
                {
                    if (!JellyMatchChecker.instance.AreJellyPartsInTouch(jellyPart.size, otherJellyPart.size,
                            direction))
                        continue;
                    if (jellyPart.type != otherJellyPart.type)
                        continue;

                    jellyPart.merge = otherJellyPart.merge = true;
                    effectedJellyControllers.Add(jelly);
                    effectedJellyControllers.Add(otherJelly);
                }
            }
        }

        public async UniTask MergeJellyPartsAsync(HashSet<JellyController> effectedJellyControllers)
        {
            var mergeCount = 0;
            var mergeTasks = new List<UniTask>();
            foreach (var effectedJellyController in effectedJellyControllers)
            {
                var effectedJellyParts = effectedJellyController.jellyParts.Where(x => x.merge).ToList();
                    effectedJellyController.RemoveJellyParts(effectedJellyParts);
                    foreach (var effectedJellyPart in effectedJellyParts)
                    {
                        mergeTasks.Add(effectedJellyPart.DoMerge());
                        mergeCount++;
                    }
            }
            
            await UniTask.WhenAll(mergeTasks);
            OnMerge?.Invoke(mergeCount);
        }

        public List<(int row, int column)> DestroyJellyIndexes(HashSet<JellyController> effectedJellyControllers)
        {
            var destroyedJellyControllers = effectedJellyControllers.Where(x => x.jellyParts.Count == 0).ToList();
            var indexes = destroyedJellyControllers.Select(x => x.GetRowAndColumn()).ToList();
            
            effectedJellyControllers.RemoveWhere(x => destroyedJellyControllers.Contains(x));
            foreach (var destroyedJellyController in destroyedJellyControllers)
                Destroy(destroyedJellyController.gameObject);
            
            return indexes;
        }

        public List<UniTask> FillInTheBlanks(HashSet<JellyController> effectedJellyControllers)
        {
            var fillTasks = new List<UniTask>();
            foreach (var effectedJellyController in effectedJellyControllers)
            {
                if (effectedJellyController.jellyParts.Count == 0)
                    continue;
                fillTasks.Add(effectedJellyController.FillInTheBlanks());
            }

            return fillTasks;
        }

        private void OnDestroy()
        {
            lifetimeCts?.Cancel();
        }
    }
}
