using System;
using System.Collections.Generic;
using System.Threading;
using IV.Core.Pools;
using UnityEngine;

namespace IV.Core.Actions
{
    [CreateAssetMenu(menuName = "Gameplay/Action Queue", fileName = nameof(ActionQueue))]
    public class ActionQueue : ScriptableObject, IDisposable
    {
        private readonly Queue<GameAction> actions = new();
        private readonly ObjectPool<GameAction> pool = new();
        private GameAction currentAction;

        private CancellationTokenSource cancellationTokenSource;

        public bool IsBusy => currentAction != null;
        public bool IsEmpty => actions.Count == 0;

        private void OnDisable() => Dispose();

        private void StopTick()
        {
            if (cancellationTokenSource == null) return;

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        // ReSharper disable once AsyncVoidMethod
        private async void StartTick()
        {
            if (IsBusy) return;
            if (IsEmpty) return;

            var exitCancellationToken = Application.exitCancellationToken;
            var cancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(exitCancellationToken);

            cancellationTokenSource = cancellationSource;
            try
            {
                await Tick(cancellationSource.Token);
                cancellationTokenSource = null;
            }
            catch (OperationCanceledException)
            {
                if (exitCancellationToken.IsCancellationRequested) StopTick();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                cancellationSource.Dispose();
            }
        }

        private async Awaitable Tick(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Awaitable.NextFrameAsync(cancellationToken);

                if (!IsBusy)
                {
                    // run out of actions
                    if (IsEmpty) return;

                    // Start next action
                    currentAction = actions.Dequeue();
                    currentAction.Start(this);
                }

                UpdateInternal();
                
                if (currentAction?.IsCompleted ?? false)
                {
                    RecycleCurrentAction();
                }
            }
        }

        private void UpdateInternal()
        {
            try
            {
                if (!currentAction.IsCompleted)
                {
                    currentAction.Update();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                // Cancel action on exception
                try
                {
                    currentAction?.Cancel();
                }
                catch
                {
                    // Ignore
                }

                RecycleCurrentAction();
            }
        }

        public void QueueAction(GameAction action, bool canInterrupt = true)
        {
            if (canInterrupt && (currentAction?.IsInterruptible ?? false))
            {
                currentAction.Cancel();
                RecycleQueue();
            }

            actions.Enqueue(action);

            StartTick();
        }

        public void CancelActions(bool force = false)
        {
            if (force)
            {
                StopTick();
            }

            // Cancel current action if allowed
            if (currentAction != null && (force || currentAction.IsInterruptible))
            {
                currentAction.Cancel();
            }

            // Clear pending actions
            RecycleQueue();
        }

        private void RecycleQueue()
        {
            foreach (var action in actions)
            {
                RecycleAction(action);
            }

            actions.Clear();
        }

        private void RecycleCurrentAction()
        {
            var action = currentAction;
            currentAction = null;

            RecycleAction(action);
        }

        public void Dispose() => StopTick();

        public T GetAction<T>() where T : GameAction, new() => pool.Get<T>();

        public void RecycleAction(GameAction action)
        {
            if (action == null) return;

            action.Recycle(this);
            pool.Return(action);
        }
    }
}