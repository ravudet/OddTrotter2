namespace System.Collections.Generic
{
    public static class ComparerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is <see langword="null"/></exception>
        public static T Max<T>(this IComparer<T> comparer, T first, T second)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            return comparer.Compare(first, second) > 0 ? first : second;
        }
    }
}