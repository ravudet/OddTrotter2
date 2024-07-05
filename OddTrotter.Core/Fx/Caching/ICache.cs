namespace Fx.Caching
{
    public interface ICache<T>
    {
        bool TryGetValue(object key, out T value);

        void CreateEntry(object key, T value);

        void Remove(object key);
    }

    public interface IObjectCache : ICache<object>
    {
    }

    public interface INullableObjectCache : ICache<object?>
    {
    }

    public sealed class Cache<T> : ICache<T>
    {
        public void CreateEntry(object key, T value)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(object key, out T value)
        {
            throw new System.NotImplementedException();
        }
    }

    public static class Driver
    {
        public static void DoWork()
        {
            var total = 0;

            var cache1 = new Cache<string>();
            cache1.TryGetValue("ASdf", out var theValue);
            total += theValue.Length;

            var cache2 = new Cache<string?>();
            cache2.TryGetValue("asdf", out var anotherValue);
            total += anotherValue.Length;
        }
    }
}
