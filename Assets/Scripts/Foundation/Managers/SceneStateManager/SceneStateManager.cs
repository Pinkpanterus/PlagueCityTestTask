using UnityEngine;
using System.Collections.Generic;

namespace Foundation
{
    sealed class SceneStateManager : AbstractService<ISceneStateManager>, ISceneStateManager
    {
        public List<SceneState> InitialStates;

        public ISceneState CurrentState { get; private set; }
        readonly List<ISceneState> states = new List<ISceneState>();
        readonly List<ISceneState> statesCache = new List<ISceneState>();
        bool statesListChanged;

        public override void Start()
        {
            base.Start();
            foreach (var state in InitialStates)
                Push(state);
        }

        public void Push(ISceneState state)
        {
            DebugOnly.Check(!states.Contains(state), $"GameState is already on the stack.");

            states.Add(state);
            statesListChanged = true;
            (state as ISceneStateInternal)?.InternalActivate();
        }

        public void Pop(ISceneState state)
        {
            statesListChanged = true;
            states.Remove(state);

            if (CurrentState == state) {
                (CurrentState as ISceneStateInternal)?.InternalResignTopmost();
                CurrentState = null;
            }

            (state as ISceneStateInternal)?.InternalDeactivate();

            DebugOnly.Check(states.Count != 0, "GameState stack is empty.");
        }

        IEnumerable<ISceneState> CachedGameStates()
        {
            int n;
            if (!statesListChanged)
                n = statesCache.Count;
            else {
                statesListChanged = false;
                statesCache.Clear();
                statesCache.AddRange(states);
                n = statesCache.Count;

                if (n == 0) {
                    if (CurrentState != null) {
                        (CurrentState as ISceneStateInternal)?.InternalResignTopmost();
                        CurrentState = null;
                    }
                } else {
                    if (CurrentState != statesCache[n - 1]) {
                        var oldState = CurrentState;
                        CurrentState = statesCache[n - 1];
                        (CurrentState as ISceneStateInternal)?.InternalBecomeTopmost();
                        if (oldState != null)
                            (oldState as ISceneStateInternal)?.InternalResignTopmost();
                    }
                }

                int index = 0;
                foreach (var it in statesCache)
                    (it as ISceneStateInternal)?.InternalSetSortingOrder(index++);
            }

            while (n-- > 0) {
                var state = statesCache[n];
                yield return state;
            }
        }

        void Update()
        {
            float timeDelta = Time.deltaTime;
            bool update = true;

            foreach (var state in CachedGameStates()) {
                if (update) {
                    foreach (var ticker in state.OnUpdate.Enumerate())
                        ticker.Do(timeDelta);
                } else {
                    foreach (var ticker in state.OnUpdateDuringPause.Enumerate())
                        ticker.Do(timeDelta);
                }

                update = update && state.UpdateParentState;
            }
        }

        void FixedUpdate()
        {
            foreach (var state in CachedGameStates()) {
                foreach (var ticker in state.OnFixedUpdate.Enumerate())
                    ticker.Do();
                if (!state.UpdateParentState)
                    break;
            }
        }

        void LateUpdate()
        {
            float timeDelta = Time.deltaTime;
            foreach (var state in CachedGameStates()) {
                foreach (var ticker in state.OnLateUpdate.Enumerate())
                    ticker.Do(timeDelta);
                if (!state.UpdateParentState)
                    break;
            }
        }
    }
}
