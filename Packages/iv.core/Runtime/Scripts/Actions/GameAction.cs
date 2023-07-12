namespace IV.Core.Actions
{
    /// <summary>
    /// Actions is a way to encapsulate a task that can be executed by the ActionQueue.
    /// Actions can be chained together to create complex behaviors.
    /// Actions can be interrupted by other actions.
    /// Actions are being pooled and recycled by the ActionQueue.
    /// </summary>
    public abstract class GameAction
    {
        public bool IsInterruptible { get; protected set; } = true;
        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// Prepapre the action to be executed
        /// </summary>
        /// <param name="actionQueue">AQ instance can be used to schedule other tasks</param>
        public abstract void Start(ActionQueue actionQueue);
        public abstract void Update();
        public virtual void Cancel() { IsCompleted = true; }

        public virtual void Recycle(ActionQueue _)
        {
            IsCompleted = false;
        }
    }
}