using Zenject;
using Foundation;

namespace Game
{
    public sealed class VisitorAvatarFactory : MonoInstaller
    {
        public int PoolSize = 10;
        public VisitorAvatar Prefab;

        public override void InstallBindings()
        {
            Container.BindFactory<VisitorAvatar, VisitorAvatar.Factory>()
                .FromMonoPoolableMemoryPool<VisitorAvatar>(opts => opts
                    .WithInitialSize(PoolSize)
                    .FromComponentInNewPrefab(Prefab)
                    .UnderTransform(transform));
        }
    }
}
