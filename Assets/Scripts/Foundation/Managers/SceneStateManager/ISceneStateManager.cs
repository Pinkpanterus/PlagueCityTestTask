namespace Foundation
{
    public interface ISceneStateManager
    {
        ISceneState CurrentState { get; }
        void Push(ISceneState state);
        void Pop(ISceneState state);
    }
}
