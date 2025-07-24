namespace H00N.Resources.Pools
{
    public interface IPoolableBehaviour
    {
        public PoolReference PoolReference { get; }

        public void OnSpawned();
        public void OnDespawn();
    }
}