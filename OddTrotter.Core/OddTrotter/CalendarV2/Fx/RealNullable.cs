/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx
{
    public readonly struct RealNullable<T>
    {
        private readonly T value;

        private readonly bool hasValue;

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="value"></param>
        public RealNullable(T value)
        {
            this.value = value;

            this.hasValue = true;
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(out T value)
        {
            value = this.value;
            return this.hasValue;
        }
    }
}
