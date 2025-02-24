namespace System.Runtime.CompilerServices
{
    public sealed class ConfiguredAwaitableWrapper<T> : IConfiguredAwaitable<T>
    {
        private readonly ConfiguredTaskAwaitable<T> configuredTaskAwaitable;

        public ConfiguredAwaitableWrapper(ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
        {
            this.configuredTaskAwaitable = configuredTaskAwaitable;
        }

        public ITaskAwaiter<T> GetAwaiter()
        {
            return new ConfiguredTaskAwaiterWrapper<T>(this.configuredTaskAwaitable.GetAwaiter());
        }
    }
}
