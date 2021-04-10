using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Zenject;

namespace Foundation
{
    public sealed class SceneManager : AbstractService<ISceneManager>, ISceneManager
    {
        public ObserverList<IOnBeginSceneLoad> OnBeginSceneLoad { get; } = new ObserverList<IOnBeginSceneLoad>();
        public ObserverList<IOnCurrentSceneUnload> OnCurrentSceneUnload { get; } = new ObserverList<IOnCurrentSceneUnload>();
        public ObserverList<IOnSceneLoadProgress> OnSceneLoadProgress { get; } = new ObserverList<IOnSceneLoadProgress>();
        public ObserverList<IOnEndSceneLoad> OnEndSceneLoad { get; } = new ObserverList<IOnEndSceneLoad>();

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        IEnumerator LoadSceneCoroutine(string sceneName)
        {
            foreach (var observer in OnBeginSceneLoad.Enumerate()) {
                var task = observer.Do();
                yield return new WaitUntil(() => task.IsCompleted);
            }

            foreach (var observer in OnCurrentSceneUnload.Enumerate())
                observer.Do();

            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone) {
                yield return null;
                foreach (var observer in OnSceneLoadProgress.Enumerate())
                    observer.Do(operation.progress);
            }

            foreach (var observer in OnEndSceneLoad.Enumerate()) {
                var task = observer.Do();
                yield return new WaitUntil(() => task.IsCompleted);
            }
        }
    }
}
